using System.Linq.Expressions;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Shared;

namespace MyApp.Tests.Integration.Utilities.Arrange;

public static class DbContextUserExtensions
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
        var user = UserEntity.Create(instanceId: 1, username, password, email);
        dbContext.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public static async Task ArrangeExpireAllConfirmations(this IBaseDbContext dbContext, int userId)
    {
        await dbContext.ArrangeExpireEntities<UserConfirmationEntity>(userId);
        await dbContext.ArrangeExpireEntities<PasswordResetConfirmationEntity>(userId);
        await dbContext.ArrangeExpireEntities<EmailChangeConfirmationEntity>(userId);
    }

    public static async Task ArrangeExpireEntities<TEntity>(this IBaseDbContext dbContext, int userId) 
        where TEntity : class, IOwnedByUser, ICreationAudited
        => await dbContext.ArrangeExpireEntities<TEntity>(x => x.UserId == userId);

    public static async Task ArrangeExpireEntities<TEntity>(this IBaseDbContext dbContext, Expression<Func<TEntity, bool>> predicate) where TEntity : class, ICreationAudited
    {
        var expiredDateTime = DateTime.UtcNow.AddYears(-1);
        await dbContext.Set<TEntity>()
            .IgnoreQueryFilters()
            .Where(predicate)
            .ExecuteUpdateAsync(x => x.SetProperty(uc => uc.CreatedAt, expiredDateTime));
    }

    public static async Task ConfirmUser(this IBaseDbContext dbContext, UserEntity user)
    {
        user.ConfirmUserRegistration();
        dbContext.Remove(user.UserConfirmation!);
        await dbContext.SaveChangesAsync();
    }
}
