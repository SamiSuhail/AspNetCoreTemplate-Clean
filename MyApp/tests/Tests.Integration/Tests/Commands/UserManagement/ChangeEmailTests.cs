using MyApp.Application.Handlers.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;

namespace MyApp.Tests.Integration.Tests.Commands.UserManagement;

public class ChangeEmailTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private readonly ChangeEmailRequest _request = new(RandomData.Email);

    [Fact]
    public async Task GivenHappyPath_ThenStoresChangeEmailConfirmation()
    {
        // Act
        var response = await AppClient.ChangeEmail(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.NewEmail.Should().Be(_request.Email);
        user.EmailChangeConfirmation.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenHappyPath_ThenExistingConfirmationIsOverwritten()
    {
        // Arrange
        var emailChangeConfirmation = EmailChangeConfirmationEntity.Create(User.Entity.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync();

        // Act
        var response = await AppClient.ChangeEmail(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.Id.Should().NotBe(emailChangeConfirmation.Id);
        user.EmailChangeConfirmation.NewEmail.Should().Be(_request.Email);
    }

    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await AppClient.ChangeEmail(_request);

        // Assert
        response.AssertSuccess();
        MockBag.AssertProduced<ChangeEmailMessage>();
    }

    [Fact]
    public async Task GivenDatabaseError_ThenNoMessageProduced()
    {
        // Arrange
        var client = AppFactory.ArrangeClientWithCredentials(userId: int.MaxValue, User.Entity.Username, User.Entity.Email);

        // Act
        var response = await client.ChangeEmail(_request);

        // Assert
        response.AssertInternalServerError();
        MockBag.AssertProduced<ChangeEmailMessage>(Times.Never());
    }

    [Fact]
    public async Task GivenMessageProducerError_ThenNoDataIsSaved()
    {
        // Arrange
        MockBag.ArrangeMessageThrows<ChangeEmailMessage>();

        // Act
        var response = await AppClient.ChangeEmail(_request);

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenMessageProducerError_ThenNoDataIsDeleted()
    {
        // Arrange
        var emailChangeConfirmation = EmailChangeConfirmationEntity.Create(User.Entity.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync();
        MockBag.ArrangeMessageThrows<ChangeEmailMessage>();

        // Act
        var response = await AppClient.ChangeEmail(_request);

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.Id.Should().Be(emailChangeConfirmation.Id);
        user.EmailChangeConfirmation.NewEmail.Should().Be(emailChangeConfirmation.NewEmail);
        user.EmailChangeConfirmation.OldEmailCode.Should().Be(emailChangeConfirmation.OldEmailCode);
        user.EmailChangeConfirmation.NewEmailCode.Should().Be(emailChangeConfirmation.NewEmailCode);
    }

    [Fact]
    public async Task GivenEmailIsIdentical_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with
        {
            Email = User.Entity.Email,
        };

        // Act
        var response = await AppClient.ChangeEmail(request);

        // Assert
        response.AssertSingleBadRequestError(EmailIsIdenticalFailure.Key, EmailIsIdenticalFailure.Message);
        await AssertNoChangesOccured();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenEmailTaken_ReturnsInvalidFailure(bool existingUserIsConfirmed)
    {
        // Arrange
        var _ = existingUserIsConfirmed 
            ? await ArrangeDbContext.ArrangeConfirmedUser(RandomData.Username, RandomData.Password, _request.Email)
            : await ArrangeDbContext.ArrangeUnconfirmedUser(RandomData.Username, RandomData.Password, _request.Email);

        // Act
        var response = await AppClient.ChangeEmail(_request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.EmailTakenKey, UserConflictFailure.EmailTakenMessage);
        await AssertNoChangesOccured();
    }

    private async Task AssertNoChangesOccured()
    {
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.EmailChangeConfirmation.Should().BeNull();
        MockBag.AssertProduced<ChangeEmailMessage>(Times.Never());
    }
}
