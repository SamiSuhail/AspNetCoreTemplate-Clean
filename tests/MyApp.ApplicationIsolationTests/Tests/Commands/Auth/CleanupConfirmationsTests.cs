using MyApp.Server.Application.Commands.Auth.BackgroundJobs.CleanupConfirmations;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.User;

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
        var user = UserEntity.Create("UnusedUsername", "UnusedPassword", "unused@email.com");
        ArrangeDbContext.Add(user);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var passwordResetConfirmation = PasswordResetConfirmationEntity.Create(user.Id);
        ArrangeDbContext.Add(passwordResetConfirmation);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var userId = user.Id;
        return userId;
    }

    private async Task ExpireConfirmations(int userId)
    {
        var timeFiveSecondsAgo = DateTime.UtcNow.AddSeconds(-5);
        var expiredEmailConfirmationDatetime = timeFiveSecondsAgo.AddMinutes(-EmailConfirmationConstants.ExpirationTimeMinutes);
        var expiredPasswordResetConfirmationDatetime = timeFiveSecondsAgo.AddMinutes(-PasswordResetConfirmationConstants.ExpirationTimeMinutes);

        var ecRowsUpdated = await ArrangeDbContext.Set<EmailConfirmationEntity>()
            .Where(ec => ec.UserId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(ec => ec.CreatedAt, expiredEmailConfirmationDatetime));
        var prcRowsUpdated = await ArrangeDbContext.Set<PasswordResetConfirmationEntity>()
            .Where(ec => ec.UserId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(ec => ec.CreatedAt, expiredPasswordResetConfirmationDatetime));

        using (new AssertionScope())
        {
            ecRowsUpdated.Should().Be(1);
            prcRowsUpdated.Should().Be(1);
        }
    }

    private async Task InvokeCleanupConfirmationsHandler()
    {
        await ScopedServices.GetRequiredService<ISender>()
                    .Send(new CleanupConfirmationsRequest());
    }

    private async Task AssertConfirmationsCleanedUp(int userId)
    {
        var ecIsDeleted = !await AssertDbContext.Set<EmailConfirmationEntity>()
                    .AnyAsync(ec => ec.UserId == userId);
        var prcIsDeleted = !await AssertDbContext.Set<PasswordResetConfirmationEntity>()
            .AnyAsync(prc => prc.UserId == userId);

        using (new AssertionScope())
        {
            ecIsDeleted.Should().BeTrue();
            prcIsDeleted.Should().BeTrue();
        }
    }
}
