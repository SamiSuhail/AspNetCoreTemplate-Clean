using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Server.Domain.Shared;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities.Arrange;

public static class UserDbContextExtensions
{
    public static async Task<(UserEntity User, string Password)> ArrangeRandomConfirmedUserWithPassword(this IBaseDbContext dbContext)
    {
        var result = await dbContext.ArrangeRandomUnconfirmedUserWithPassword();
        await dbContext.ConfirmUser(result.User);
        return result;
    }

    public static async Task<UserEntity> ArrangeRandomConfirmedUser(this IBaseDbContext dbContext)
    {
        var user = await dbContext.ArrangeRandomUnconfirmedUser();
        await dbContext.ConfirmUser(user);
        return user;
    }

    public static async Task<UserEntity> ArrangeConfirmedUser(this IBaseDbContext dbContext, string username, string password, string email)
    {
        var user = await dbContext.ArrangeUnconfirmedUser(username, password, email);
        await dbContext.ConfirmUser(user);
        return user;
    }

    public static async Task<(UserEntity User, string Password)> ArrangeRandomUnconfirmedUserWithPassword(this IBaseDbContext dbContext)
    {
        var password = RandomData.Password;
        var user = await dbContext.ArrangeUnconfirmedUser(RandomData.Username, password, RandomData.Email);
        return (user, password);
    }

    public static async Task<UserEntity> ArrangeRandomUnconfirmedUser(this IBaseDbContext dbContext)
        => await dbContext.ArrangeUnconfirmedUser(RandomData.Username, RandomData.Password, RandomData.Email);

    public static async Task<UserEntity> ArrangeUnconfirmedUser(this IBaseDbContext dbContext, string username, string password, string email)
    {
        var user = UserEntity.Create(username, password, email);
        dbContext.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    public static async Task ArrangeExpireAllConfirmations(this IBaseDbContext dbContext, int userId)
    {
        var timeFiveSecondsAgo = DateTime.UtcNow.AddSeconds(-5);
        var expiredDatetime = timeFiveSecondsAgo.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);

        await ExpireConfirmations<UserConfirmationEntity>(userId, expiredDatetime);
        await ExpireConfirmations<PasswordResetConfirmationEntity>(userId, expiredDatetime);
        await ExpireConfirmations<EmailChangeConfirmationEntity>(userId, expiredDatetime);

        async Task ExpireConfirmations<TConfirmationEntity>(int userId, DateTime expiredDatetime) where TConfirmationEntity : class, IOwnedByUser, ICreationAudited
        {
            await dbContext.Set<TConfirmationEntity>()
                .Where(uc => uc.UserId == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(uc => uc.CreatedAt, expiredDatetime));
        }
    }

    public static async Task ConfirmUser(this IBaseDbContext dbContext, UserEntity user)
    {
        user.ConfirmUserRegistration();
        dbContext.Remove(user.UserConfirmation!);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
}
