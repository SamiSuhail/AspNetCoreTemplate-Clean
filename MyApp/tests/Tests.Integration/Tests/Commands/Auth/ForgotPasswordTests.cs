using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Tests.Integration.Tests.Commands.Auth;

public class ForgotPasswordTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private UserEntity _user = default!;
    private ForgotPasswordRequest _request = default!;

    public override async Task DisposeAsync()
    {
        var dbContext = CreateDbContext();
        await dbContext.Set<PasswordResetConfirmationEntity>()
            .Where(pr => pr.UserId == _user.Id)
            .ExecuteDeleteAsync();
        await base.DisposeAsync();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPath_ThenStoresPasswordResetConfirmation(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(_request);

        // Assert
        response.AssertSuccess();
        var passwordReset = await GetPasswordReset();
        passwordReset.Should().NotBeNull();
        using var _ = new AssertionScope();
        passwordReset!.Code.Should().NotBeNullOrWhiteSpace();
        passwordReset.CreatedAt.ShouldBeNow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPath_ThenProducesMessage(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(_request);

        // Assert
        response.AssertSuccess();
        MockBag.AssertProduced<SendPasswordResetConfirmationMessage>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPathPasswordConfirmationAlreadyExists_ThenStoresOnlyLatestPasswordResetConfirmation(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);
        var oldPasswordReset = PasswordResetConfirmationEntity.Create(_user.Id);
        ArrangeDbContext.Add(oldPasswordReset);
        await ArrangeDbContext.SaveChangesAsync();

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(_request);

        // Assert
        response.AssertSuccess();
        var passwordResets = await AssertDbContext.Set<PasswordResetConfirmationEntity>()
            .IgnoreQueryFilters()
            .Where(pr => pr.UserId == _user.Id)
            .ToArrayAsync();
        passwordResets.Should().HaveCount(1);
        var passwordReset = passwordResets[0];
        passwordReset.Should().NotBeNull();
        passwordReset!.Id.Should().NotBe(oldPasswordReset.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenDatabaseError_ThenNoMessageIsProduced(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);
        var request = _request with { Email = RandomData.Email };

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(request);

        // Assert
        response.AssertBadRequest();
        MockBag.AssertProduced<SendPasswordResetConfirmationMessage>(Times.Never());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenMessageProducerError_ThenNoConfirmationIsStored(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);
        MockBag.ArrangeMessageThrows<SendPasswordResetConfirmationMessage>();

        // Act
        await UnauthorizedAppClient.ForgotPassword(_request);

        // Assert
        await AssertConfirmationNotExists();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenUsernameNotExists_ReturnsInvalidFailure(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);
        var request = _request with { Username = RandomData.Username };

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(request);

        // Assert
        AssertInvalidFailure(response);
        await AssertConfirmationNotExists();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenEmailNotExists_ReturnsInvalidFailure(bool userIsConfirmed)
    {
        // Arrange
        await ArrangeUserAndRequest(userIsConfirmed);
        var request = _request with { Email = RandomData.Email };

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(request);

        // Assert
        AssertInvalidFailure(response);
        await AssertConfirmationNotExists();
    }

    private static void AssertInvalidFailure(IApiResponse response)
    {
        response.AssertSingleBadRequestError(ForgotPasswordInvalidFailure.Key, ForgotPasswordInvalidFailure.Message);
    }

    private async Task ArrangeUserAndRequest(bool userIsConfirmed)
    {
        _user = userIsConfirmed
                    ? User.Entity
                    : await ArrangeDbContext.ArrangeRandomUnconfirmedUser();
        _request = new ForgotPasswordRequest(_user.Email, _user.Username);
    }

    private async Task AssertConfirmationNotExists()
    {
        var passwordReset = await GetPasswordReset();
        passwordReset.Should().BeNull();
    }

    private async Task<PasswordResetConfirmationEntity?> GetPasswordReset()
    {
        return await AssertDbContext.Set<PasswordResetConfirmationEntity>()
            .IgnoreQueryFilters()
            .Where(pr => pr.UserId == _user.Id)
            .FirstOrDefaultAsync();
    }
}
