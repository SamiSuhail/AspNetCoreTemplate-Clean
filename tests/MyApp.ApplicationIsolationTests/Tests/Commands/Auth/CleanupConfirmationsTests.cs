using MyApp.Server.Application.Commands.BackgroundJobs.CleanupConfirmations;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Shared;

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
        var emailChangeConfirmationEntity = EmailChangeConfirmationEntity.Create(user.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmationEntity);
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
            CheckIsDeleted<EmailChangeConfirmationEntity>(userId));

        results.All(r => r == true).Should().BeTrue();

        async Task<bool> CheckIsDeleted<TConfirmationEntity>(int userId) where TConfirmationEntity : class, IOwnedByUser
        {
            return !await CreateDbContext().Set<TConfirmationEntity>()
                        .AnyAsync(uc => uc.UserId == userId);
        }
    }
}
