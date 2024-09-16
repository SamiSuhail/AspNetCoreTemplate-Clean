using MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Domain.UserManagement.EmailChangeConfirmation.Failures;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.UserManagement;

public class ChangeEmailTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private UserEntity _user = default!;
    private readonly ChangeEmailRequest _request = new(RandomData.Email);
    private IApplicationClient _client = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomConfirmedUser();
        _client = AppFactory.CreateClientWithCredentials(_user.Id, _user.Username, _user.Email);
    }

    [Fact]
    public async Task GivenHappyPath_ThenStoresChangeEmailConfirmation()
    {
        // Act
        var response = await _client.ChangeEmail(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.NewEmail.Should().Be(_request.Email);
        user.EmailChangeConfirmation.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenHappyPath_ThenExistingConfirmationIsOverwritten()
    {
        // Arrange
        var emailChangeConfirmation = EmailChangeConfirmationEntity.Create(_user.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var response = await _client.ChangeEmail(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.Id.Should().NotBe(emailChangeConfirmation.Id);
        user.EmailChangeConfirmation.NewEmail.Should().Be(_request.Email);
    }

    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await _client.ChangeEmail(_request);

        // Assert
        response.AssertSuccess();
        AssertMessage.Produced<ChangeEmailMessage>();
    }

    [Fact]
    public async Task GivenDatabaseError_ThenNoMessageProduced()
    {
        // Arrange
        _client = AppFactory.CreateClientWithCredentials(userId: int.MaxValue, _user.Username, _user.Email);

        // Act
        var response = await _client.ChangeEmail(_request);

        // Assert
        response.AssertInternalServerError();
        AssertMessage.Produced<ChangeEmailMessage>(Times.Never());
    }

    [Fact]
    public async Task GivenMessageProducerError_ThenNoDataIsSaved()
    {
        // Arrange
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<ChangeEmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var response = await _client.ChangeEmail(_request);

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenMessageProducerError_ThenNoDataIsDeleted()
    {
        // Arrange
        var emailChangeConfirmation = EmailChangeConfirmationEntity.Create(_user.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<ChangeEmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var response = await _client.ChangeEmail(_request);

        // Assert
        response.AssertInternalServerError();
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().NotBeNull();
        user.EmailChangeConfirmation!.Id.Should().Be(emailChangeConfirmation.Id);
        user.EmailChangeConfirmation.NewEmail.Should().Be(emailChangeConfirmation.NewEmail);
        user.EmailChangeConfirmation.OldEmailCode.Should().Be(emailChangeConfirmation.OldEmailCode);
        user.EmailChangeConfirmation.NewEmailCode.Should().Be(emailChangeConfirmation.NewEmailCode);
    }

    [Fact]
    public async Task GivenUnauthorized_ReturnsUnauthorizedError()
    {
        // Act
        var response = await UnauthorizedAppClient.ChangeEmail(_request);

        // Assert
        response.AssertUnauthorizedError();
        await AssertNoChangesOccured();
    }

    [Fact]
    public async Task GivenEmailIsIdentical_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with
        {
            Email = _user.Email,
        };

        // Act
        var response = await _client.ChangeEmail(request);

        // Assert
        response.AssertSingleBadRequestError(EmailIsIdenticalFailure.Key, EmailIsIdenticalFailure.Message);
        await AssertNoChangesOccured();
    }

    [Fact]
    public async Task GivenEmailIsAlreadyInUse_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with
        {
            Email = TestUser.Email,
        };

        // Act
        var response = await _client.ChangeEmail(request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.EmailTakenKey, UserConflictFailure.EmailTakenMessage);
        await AssertNoChangesOccured();
    }

    private async Task AssertNoChangesOccured()
    {
        var user = await AssertDbContext.GetUser(_user.Id);
        user.EmailChangeConfirmation.Should().BeNull();
        AssertMessage.Produced<ChangeEmailMessage>(Times.Never());
    }
}
