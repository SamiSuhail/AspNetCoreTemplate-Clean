using MyApp.Server.Application.Commands.BackgroundJobs.CleanupConfirmations;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Shared;
using MyApp.Server.Domain.Shared.Confirmations;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class CleanupConfirmationsTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    [Fact]
    public async Task GivenHappyPath_ThenConfirmationsAreDeleted()
    {
        // Arrange
        int userId = await CreateUserAndConfirmations();
        await ExpireConfirmations(userId);

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

    private async Task ExpireConfirmations(int userId)
    {
        var timeFiveSecondsAgo = DateTime.UtcNow.AddSeconds(-5);
        var expiredDatetime = timeFiveSecondsAgo.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);

        var results = await Task.WhenAll(
            ExpireConfirmations<UserConfirmationEntity>(userId, expiredDatetime),
            ExpireConfirmations<PasswordResetConfirmationEntity>(userId, expiredDatetime),
            ExpireConfirmations<EmailChangeConfirmationEntity>(userId, expiredDatetime)
            );

        results.All(x => x == 1).Should().BeTrue();

        async Task<int> ExpireConfirmations<TConfirmationEntity>(int userId, DateTime expiredDatetime) where TConfirmationEntity : class, IOwnedByUser, ICreationAudited
        {
            return await CreateDbContext().Set<TConfirmationEntity>()
                .Where(uc => uc.UserId == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(uc => uc.CreatedAt, expiredDatetime));
        }
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
