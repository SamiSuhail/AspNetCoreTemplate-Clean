using MyApp.Server.Application.Commands.BackgroundJobs.CleanupConfirmations;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Shared;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class CleanupConfirmationsTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    [Fact]
    public async Task GivenHappyPath_ThenConfirmationsAreDeleted()
    {
        // Arrange
        int userId = await CreateUserAndConfirmations();
        await ArrangeDbContext.ArrangeExpireAllConfirmations(userId);

        // Act
        await InvokeCleanupConfirmationsHandler();

        // Assert
        await AssertConfirmationsCleanedUp(userId);
    }

    private async Task<int> CreateUserAndConfirmations()
    {
        var user = await ArrangeDbContext.ArrangeRandomUnconfirmedUser();

        var passwordResetConfirmation = PasswordResetConfirmationEntity.Create(user.Id);
        ArrangeDbContext.Add(passwordResetConfirmation);

        var emailChangeConfirmation = EmailChangeConfirmationEntity.Create(user.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmation);

        var passwordChangeConfirmation = PasswordChangeConfirmationEntity.Create(user.Id, RandomData.Password);
        ArrangeDbContext.Add(passwordChangeConfirmation);

        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var userId = user.Id;
        return userId;
    }

    private async Task InvokeCleanupConfirmationsHandler()
    {
        await ScopedServices.GetRequiredService<ISender>()
                    .Send(new CleanupConfirmationsRequest());
    }

    private async Task AssertConfirmationsCleanedUp(int userId)
    {
        var results = await Task.WhenAll(
            CheckIsDeleted<UserConfirmationEntity>(userId),
            CheckIsDeleted<PasswordResetConfirmationEntity>(userId),
            CheckIsDeleted<EmailChangeConfirmationEntity>(userId),
            CheckIsDeleted<PasswordChangeConfirmationEntity>(userId));

        results.All(r => r == true).Should().BeTrue();

        async Task<bool> CheckIsDeleted<TConfirmationEntity>(int userId) where TConfirmationEntity : class, IOwnedByUser
        {
            return !await CreateDbContext().Set<TConfirmationEntity>()
                        .AnyAsync(uc => uc.UserId == userId);
        }
    }
}
