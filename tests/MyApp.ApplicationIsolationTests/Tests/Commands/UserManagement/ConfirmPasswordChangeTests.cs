using MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation.Failures;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.UserManagement;

public class ConfirmPasswordChangeTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private ConfirmPasswordChangeRequest _request = default!;
    private PasswordChangeConfirmationEntity _passwordChangeConfirmation = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var password = RandomData.Password;
        _passwordChangeConfirmation = PasswordChangeConfirmationEntity.Create(User.Entity.Id, password);
        ArrangeDbContext.Add(_passwordChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        _request = new(_passwordChangeConfirmation.Code);
    }

    [Fact]
    public async Task GivenHappyPath_ThenChangesPassword()
    {
        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordHash.Should().Be(_passwordChangeConfirmation.NewPasswordHash);
    }

    [Fact]
    public async Task GivenHappyPath_ThenDeletesConfirmationEntity()
    {
        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenHappyPath_ThenIncrementsRefreshTokenVersion()
    {
        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

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
        var response = await client.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSingleBadRequestError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
        await AssertNoDataChanges();
    }

    [Fact]
    public async Task GivenNoPasswordChangeRequested_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.Set<PasswordChangeConfirmationEntity>()
            .Where(ecc => ecc.Id == _passwordChangeConfirmation.Id)
            .ExecuteDeleteAsync();

        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSingleBadRequestError(PasswordChangeNotRequestedFailure.Key, PasswordChangeNotRequestedFailure.Message);
    }

    [Fact]
    public async Task GivenPasswordChangeExpired_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ArrangeExpireAllConfirmations(User.Entity.Id);

        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSingleBadRequestError(PasswordChangeExpiredFailure.Key, PasswordChangeExpiredFailure.Message);
        await AssertNoDataChanges();
    }

    [Fact]
    public async Task GivenCodeInvalid_ReturnsInvalidFailure()
    {
        // Arrange
        _request = _request with
        {
            Code = "000000",
        };

        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSingleBadRequestError(PasswordChangeInvalidCodeFailure.Key, PasswordChangeInvalidCodeFailure.Message);
        await AssertNoDataChanges();
    }

    private async Task AssertNoDataChanges()
    {
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().NotBeNull();
        user.PasswordChangeConfirmation!.Id.Should().Be(_passwordChangeConfirmation.Id);
        user.RefreshTokenVersion.Should().Be(User.Entity.RefreshTokenVersion);
    }
}
