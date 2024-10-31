using MyApp.Application.Modules.BackgroundJobs.Cleanup.Confirmations;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Shared;

namespace MyApp.Tests.Integration.Tests.Features.Cleanup;

public class CleanupConfirmationsTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    [Fact]
    public async Task GivenHappyPath_ThenConfirmationsAreDeleted()
    {
        // Arrange
        int userId = await CreateUserAndConfirmations();
        await ArrangeDbContext.ArrangeExpireAllConfirmations(userId);

        // Act
        await ScopedServices.GetRequiredService<ISender>()
            .Send(new CleanupConfirmationsRequest());

        // Assert
        await AssertConfirmationsCleanedUp(userId);
    }

    [Fact]
    public async Task GivenConfirmationsNotExpired_ThenConfirmationsAreNotDeleted()
    {
        // Arrange
        int userId = await CreateUserAndConfirmations();

        // Act
        await ScopedServices.GetRequiredService<ISender>()
            .Send(new CleanupConfirmationsRequest());

        // Assert
        await AssertConfirmationsNotCleanedUp(userId);
    }

    private async Task<int> CreateUserAndConfirmations()
    {
        var user = await ArrangeDbContext.ArrangeRandomUnconfirmedUser();
        var passwordResetConfirmation = PasswordResetConfirmationEntity.Create(user.Id);
        ArrangeDbContext.Add(passwordResetConfirmation);
        var emailChangeConfirmationEntity = EmailChangeConfirmationEntity.Create(user.Id, RandomData.Email);
        ArrangeDbContext.Add(emailChangeConfirmationEntity);
        await ArrangeDbContext.SaveChangesAsync();
        var userId = user.Id;
        return userId;
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
                .IgnoreQueryFilters()
                .AnyAsync(uc => uc.UserId == userId);
        }
    }

    private async Task AssertConfirmationsNotCleanedUp(int userId)
    {
        var results = await Task.WhenAll(
            CheckIsNotDeleted<UserConfirmationEntity>(userId),
            CheckIsNotDeleted<PasswordResetConfirmationEntity>(userId),
            CheckIsNotDeleted<EmailChangeConfirmationEntity>(userId));

        results.All(r => r == false).Should().BeTrue();

        async Task<bool> CheckIsNotDeleted<TConfirmationEntity>(int userId) where TConfirmationEntity : class, IOwnedByUser
        {
            return !await CreateDbContext().Set<TConfirmationEntity>()
                .IgnoreQueryFilters()
                .AnyAsync(uc => uc.UserId == userId);
        }
    }
}
