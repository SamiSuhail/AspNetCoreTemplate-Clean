using MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ChangePassword;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation.Failures;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.UserManagement;

public class ChangePasswordTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private ChangePasswordRequest _request = new(RandomData.Password);

    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await AppClient.ChangePassword(_request);

        // Assert
        response.AssertSuccess();
        MockBag.AssertProduced<ChangePasswordMessage>();
    }

    [Fact]
    public async Task GivenHappyPath_ThenStoresConfirmation()
    {
        // Act
        var response = await AppClient.ChangePassword(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().NotBeNull();
        using var _ = new AssertionScope();
        _request.NewPassword.Verify(user.PasswordChangeConfirmation!.NewPasswordHash).Should().BeTrue();
        user.PasswordChangeConfirmation.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenHappyPath_ThenOverwritesExistingConfirmation()
    {
        // Arrange
        var existingConfirmation = await ArrangeExistingConfirmation();

        // Act
        var response = await AppClient.ChangePassword(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().NotBeNull();
        using var _ = new AssertionScope();
        user.PasswordChangeConfirmation!.Id.Should().NotBe(existingConfirmation.Id);
        user.PasswordChangeConfirmation.Code.Should().NotBe(existingConfirmation.Code);
        user.PasswordChangeConfirmation.NewPasswordHash.Should().NotBe(existingConfirmation.NewPasswordHash);
        user.PasswordChangeConfirmation.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenUserIsUnauthrozied_ReturnsUnauthorizedError()
    {
        // Act
        var response = await UnauthorizedAppClient.ChangePassword(_request);

        // Assert
        response.AssertUnauthorizedError();
        await AssertNoChangesOccured();
    }

    [Fact]
    public async Task GivenUserIdNotFound_ReturnsInvalidFailure()
    {
        // Arrange
        var client = AppFactory.ArrangeClientWithCredentials(userId: int.MaxValue, User.Entity.Username, User.Entity.Email);

        // Act
        var response = await client.ChangePassword(_request);

        // Assert
        response.AssertSingleBadRequestError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
        await AssertNoChangesOccured();
    }

    [Fact]
    public async Task GivenPasswordIdentical_ReturnsInvalidFailure()
    {
        // Arrange
        _request = _request with { NewPassword = User.Password };

        // Act
        var response = await AppClient.ChangePassword(_request);

        // Assert
        response.AssertSingleBadRequestError(PasswordIsIdenticalFailure.Key, PasswordIsIdenticalFailure.Message);
        await AssertNoChangesOccured();
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataSaved()
    {
        // Arrange
        MockBag.ArrangeMessageThrows<ChangePasswordMessage>();

        // Act
        var response = await AppClient.ChangePassword(_request);

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataDeleted()
    {
        // Arrange
        var existingConfirmation = await ArrangeExistingConfirmation();
        MockBag.ArrangeMessageThrows<ChangePasswordMessage>();

        // Act
        var response = await AppClient.ChangePassword(_request);

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().NotBeNull();
        user.PasswordChangeConfirmation!.Id.Should().Be(existingConfirmation.Id);
    }

    private async Task<PasswordChangeConfirmationEntity> ArrangeExistingConfirmation()
    {
        var existingConfirmation = PasswordChangeConfirmationEntity.Create(User.Entity.Id, RandomData.Password);
        ArrangeDbContext.Add(existingConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);
        return existingConfirmation;
    }

    private async Task AssertNoChangesOccured()
    {
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordChangeConfirmation.Should().BeNull();
        MockBag.AssertProduced<ChangePasswordMessage>(Times.Never());
    }
}
