using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.UserManagement;

public class ConfirmEmailChangeTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private ConfirmEmailChangeRequest _request = default!;
    private EmailChangeConfirmationEntity _emailChangeConfirmation = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _emailChangeConfirmation = EmailChangeConfirmationEntity.Create(User.Entity.Id, RandomData.Email);
        ArrangeDbContext.Add(_emailChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        _request = new(_emailChangeConfirmation.OldEmailCode, _emailChangeConfirmation.NewEmailCode);
    }

    [Fact]
    public async Task GivenHappyPath_ThenChangesEmail()
    {
        // Act
        var response = await AppClient.ConfirmEmailChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.Email.Should().Be(_emailChangeConfirmation.NewEmail);
    }

    [Fact]
    public async Task GivenHappyPath_ThenDeletesConfirmationEntity()
    {
        // Act
        var response = await AppClient.ConfirmEmailChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenHappyPath_ThenIncrementsRefreshTokenVersion()
    {
        // Act
        var response = await AppClient.ConfirmEmailChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.RefreshTokenVersion.Should().Be(User.Entity.RefreshTokenVersion + 1);
    }

    [Fact]
    public async Task GivenUserNotExists_ReturnsInvalidFailure()
    {
        // Arrange
        var client = AppFactory.ArrangeClientWithCredentials(userId: int.MaxValue, User.Entity.Username, User.Entity.Email);

        // Act
        var response = await client.ConfirmEmailChange(_request);

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
        var response = await AppClient.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(EmailChangeNotRequestedFailure.Key, EmailChangeNotRequestedFailure.Message);
    }

    [Fact]
    public async Task GivenEmailChangeExpired_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ArrangeExpireAllConfirmations(User.Entity.Id);

        // Act
        var response = await AppClient.ConfirmEmailChange(_request);

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
        var response = await AppClient.ConfirmEmailChange(_request);

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
        var response = await AppClient.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(EmailChangeInvalidCodesFailure.Key, EmailChangeInvalidCodesFailure.Message);
        await AssertNoDataChanges();
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenEmailTaken_ReturnsInvalidFailure(bool existingUserIsConfirmed)
    {
        // Arrange
        var _ = existingUserIsConfirmed
            ? await ArrangeDbContext.ArrangeConfirmedUser(RandomData.Username, RandomData.Password, _emailChangeConfirmation.NewEmail)
            : await ArrangeDbContext.ArrangeUnconfirmedUser(RandomData.Username, RandomData.Password, _emailChangeConfirmation.NewEmail);

        // Act
        var response = await AppClient.ConfirmEmailChange(_request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.EmailTakenKey, UserConflictFailure.EmailTakenMessage);
        await AssertNoDataChanges();
    }

    private async Task AssertNoDataChanges()
    {
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.Id.Should().Be(_emailChangeConfirmation.Id);
        user.RefreshTokenVersion.Should().Be(User.Entity.RefreshTokenVersion);
    }
}
