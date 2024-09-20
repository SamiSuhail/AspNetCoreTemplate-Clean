using MyApp.Server.Application.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Server.Domain.Auth.User;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class ResetPasswordTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private int _userId;
    private PasswordResetConfirmationEntity _passwordReset = default!;
    private ResetPasswordRequest _request = default!;

    public override async Task DisposeAsync()
    {
        await CreateDbContext().Set<PasswordResetConfirmationEntity>()
            .Where(pr => pr.UserId == _userId)
            .ExecuteDeleteAsync();
        await base.DisposeAsync();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPath_ThenUpdatesPassword(bool userIsConfirmed)
    {
        // Arrange
        await ArrangePasswordResetAndRequest(userIsConfirmed);

        // Act
        var response = await UnauthorizedAppClient.ResetPassword(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_userId);
        _request.Password.Verify(user!.PasswordHash).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPath_ThenDeletesPasswordResetConfirmation(bool userIsConfirmed)
    {
        // Arrange
        await ArrangePasswordResetAndRequest(userIsConfirmed);

        // Act
        var response = await UnauthorizedAppClient.ResetPassword(_request);

        // Assert
        response.AssertSuccess();
        var passwordResetExists = await AssertDbContext.Set<PasswordResetConfirmationEntity>()
            .Where(pr => pr.UserId == _userId)
            .AnyAsync();
        passwordResetExists.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenCodeNotExists_ReturnsInvalidFailure(bool userIsConfirmed)
    {
        // Arrange
        await ArrangePasswordResetAndRequest(userIsConfirmed);
        var request = _request with { Code = "000000" };

        // Act
        var response = await UnauthorizedAppClient.ResetPassword(request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenCodeExpired_ReturnsInvalidFailure(bool userIsConfirmed)
    {
        // Arrange
        await ArrangePasswordResetAndRequest(userIsConfirmed);
        await ArrangeDbContext.ArrangeExpireAllConfirmations(_userId);

        // Act
        var response = await UnauthorizedAppClient.ResetPassword(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    private static void AssertInvalidFailure(IApiResponse response)
    {
        response.AssertSingleBadRequestError(PasswordResetConfirmationCodeInvalidFailure.Key, PasswordResetConfirmationCodeInvalidFailure.Message);
    }

    private async Task ArrangePasswordResetAndRequest(bool userIsConfirmed)
    {
        string password;
        if (userIsConfirmed)
        {
            password = User.Password;
            _userId = User.Entity.Id;
        }
        else
        {
            var result = await ArrangeDbContext.ArrangeRandomUnconfirmedUserWithPassword();
            password = result.Password;
            _userId = result.User.Id;
        }

        _passwordReset = PasswordResetConfirmationEntity.Create(_userId);
        ArrangeDbContext.Add(_passwordReset);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        _request = new ResetPasswordRequest(_passwordReset.Code, password);
    }
}
