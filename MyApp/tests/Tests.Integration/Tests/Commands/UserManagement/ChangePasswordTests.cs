using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Tests.Integration.Tests.Commands.UserManagement;

public class ChangePasswordTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await AppClient.ChangePassword();

        // Assert
        response.AssertSuccess();
        MockBag.AssertProduced<SendPasswordResetConfirmationMessage>();
    }

    [Fact]
    public async Task GivenHappyPath_ThenStoresConfirmation()
    {
        // Act
        var response = await AppClient.ChangePassword();

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().NotBeNull();
        using var _ = new AssertionScope();
        user.PasswordResetConfirmation!.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenHappyPath_ThenOverwritesExistingConfirmation()
    {
        // Arrange
        var existingConfirmation = await ArrangeExistingConfirmation();

        // Act
        var response = await AppClient.ChangePassword();

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().NotBeNull();
        using var _ = new AssertionScope();
        user.PasswordResetConfirmation!.Id.Should().NotBe(existingConfirmation.Id);
        user.PasswordResetConfirmation.Code.Should().NotBe(existingConfirmation.Code);
        user.PasswordResetConfirmation.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenUserIdNotFound_ReturnsInvalidFailure()
    {
        // Arrange
        var client = AppFactory.ArrangeClientWithCredentials(userId: int.MaxValue, User.Entity.Username, User.Entity.Email);

        // Act
        var response = await client.ChangePassword();

        // Assert
        response.AssertUserNotFoundFailure();
        await AssertNoChangesOccurred();
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataSaved()
    {
        // Arrange
        MockBag.ArrangeMessageThrows<SendPasswordResetConfirmationMessage>();

        // Act
        var response = await AppClient.ChangePassword();

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataDeleted()
    {
        // Arrange
        var existingConfirmation = await ArrangeExistingConfirmation();
        MockBag.ArrangeMessageThrows<SendPasswordResetConfirmationMessage>();

        // Act
        var response = await AppClient.ChangePassword();

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().NotBeNull();
        user.PasswordResetConfirmation!.Id.Should().Be(existingConfirmation.Id);
    }

    private async Task<PasswordResetConfirmationEntity> ArrangeExistingConfirmation()
    {
        var existingConfirmation = PasswordResetConfirmationEntity.Create(User.Entity.Id);
        ArrangeDbContext.Add(existingConfirmation);
        await ArrangeDbContext.SaveChangesAsync();
        return existingConfirmation;
    }

    private async Task AssertNoChangesOccurred()
    {
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.PasswordResetConfirmation.Should().BeNull();
        MockBag.AssertProduced<SendPasswordResetConfirmationMessage>(Times.Never());
    }
}
