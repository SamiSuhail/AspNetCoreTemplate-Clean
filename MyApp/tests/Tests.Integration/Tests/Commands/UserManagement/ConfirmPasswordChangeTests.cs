using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

namespace MyApp.Tests.Integration.Tests.Commands.UserManagement;

public class ConfirmPasswordChangeTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private ConfirmPasswordChangeRequest _request = default!;
    private PasswordResetConfirmationEntity _passwordResetConfirmation = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _passwordResetConfirmation = PasswordResetConfirmationEntity.Create(User.Entity.Id);
        ArrangeDbContext.Add(_passwordResetConfirmation);
        await ArrangeDbContext.SaveChangesAsync();

        _request = new(_passwordResetConfirmation.Code, RandomData.Password);
    }

    [Fact]
    public async Task GivenHappyPath_ThenChangesPassword()
    {
        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        _request.Password.Verify(user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task GivenHappyPath_ThenDeletesConfirmationEntity()
    {
        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().BeNull();
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
        response.AssertUserNotFoundFailure();
        await AssertNoDataChanges();
    }

    [Fact]
    public async Task GivenNoPasswordChangeRequested_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.Set<PasswordResetConfirmationEntity>()
            .Where(ecc => ecc.Id == _passwordResetConfirmation.Id)
            .ExecuteDeleteAsync();

        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSingleBadRequestError(PasswordResetNotRequestedFailure.Key, PasswordResetNotRequestedFailure.Message);
    }

    [Fact]
    public async Task GivenPasswordChangeExpired_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ArrangeExpireAllConfirmations(User.Entity.Id);

        // Act
        var response = await AppClient.ConfirmPasswordChange(_request);

        // Assert
        response.AssertSingleBadRequestError(PasswordResetExpiredFailure.Key, PasswordResetExpiredFailure.Message);
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
        response.AssertSingleBadRequestError(PasswordResetInvalidCodeFailure.Key, PasswordResetInvalidCodeFailure.Message);
        await AssertNoDataChanges();
    }

    private async Task AssertNoDataChanges()
    {
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().NotBeNull();
        user.PasswordResetConfirmation!.Id.Should().Be(_passwordResetConfirmation.Id);
        user.RefreshTokenVersion.Should().Be(User.Entity.RefreshTokenVersion);
    }
}
