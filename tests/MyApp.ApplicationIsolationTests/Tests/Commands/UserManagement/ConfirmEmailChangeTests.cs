using MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Domain.UserManagement.EmailChangeConfirmation.Failures;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.UserManagement;

public class ConfirmEmailChangeTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private UserEntity _user = default!;
    private ConfirmEmailChangeRequest _request = default!;
    private EmailChangeConfirmationEntity _emailChangeConfirmation = default!;
    private IApplicationClient _client = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomConfirmedUser();
        _client = AppFactory.CreateClientWithCredentials(_user.Id, _user.Username, _user.Email);

        _emailChangeConfirmation = EmailChangeConfirmationEntity.Create(_user.Id, RandomData.Email);
        ArrangeDbContext.Add(_emailChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        _request = new(_emailChangeConfirmation.OldEmailCode, _emailChangeConfirmation.NewEmailCode);
    }

    [Fact]
    public async Task GivenHappyPath_ThenChangesEmail()
    {
        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.Email.Should().Be(_emailChangeConfirmation.NewEmail);
    }

    [Fact]
    public async Task GivenHappyPath_ThenDeletesConfirmationEntity()
    {
        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenHappyPath_ThenIncrementsRefreshTokenVersion()
    {
        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.RefreshTokenVersion.Should().Be(_user.RefreshTokenVersion + 1);
    }

    [Fact]
    public async Task GivenUserNotExists_ReturnsInvalidFailure()
    {
        // Arrange
        _client = AppFactory.CreateClientWithCredentials(userId: int.MaxValue, _user.Username, _user.Email);

        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
        await AssertNoDataChanges();
    }

    [Fact]
    public async Task GivenNoEmailChangeRequested_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.Set<EmailChangeConfirmationEntity>()
            .Where(ecc => ecc.Id == _emailChangeConfirmation.Id)
            .ExecuteDeleteAsync();

        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(EmailChangeNotRequestedFailure.Key, EmailChangeNotRequestedFailure.Message);
    }

    [Fact]
    public async Task GivenEmailChangeExpired_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ArrangeExpireAllConfirmations(_user.Id);

        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(EmailChangeExpiredFailure.Key, EmailChangeExpiredFailure.Message);
        await AssertNoDataChanges();
    }

    [Fact]
    public async Task GivenOldEmailCodeInvalid_ReturnsInvalidFailure()
    {
        // Arrange
        _request = _request with
        {
            OldEmailCode = "000000",
        };

        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(EmailChangeInvalidCodesFailure.Key, EmailChangeInvalidCodesFailure.Message);
        await AssertNoDataChanges();
    }

    [Fact]
    public async Task GivenNewEmailCodeInvalid_ReturnsInvalidFailure()
    {
        // Arrange
        _request = _request with
        {
            NewEmailCode = "000000",
        };

        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(EmailChangeInvalidCodesFailure.Key, EmailChangeInvalidCodesFailure.Message);
        await AssertNoDataChanges();
    }


    [Fact]
    public async Task GivenEmailAlreadyInUse_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.Set<EmailChangeConfirmationEntity>()
            .Where(ecc => ecc.Id == _emailChangeConfirmation.Id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(ecc =>
                    ecc.NewEmail,
                    User.Entity.Email));

        // Act
        var response = await _client.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.EmailTakenKey, UserConflictFailure.EmailTakenMessage);
        await AssertNoDataChanges();
    }

    private async Task AssertNoDataChanges()
    {
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.Id.Should().Be(_emailChangeConfirmation.Id);
        user.RefreshTokenVersion.Should().Be(_user.RefreshTokenVersion);
    }
}
