using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using static MyApp.Infrastructure.Database.Constants;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Shared;

public record ConfirmationConfiguration(
    string SchemaName,
    string TableName);

public static class ConfirmationConfigurations
{
    private static readonly Dictionary<Type, ConfirmationConfiguration> _configurations = new()
    {
        { typeof(UserConfirmationEntity),  new(Schemas.Auth, Tables.UserConfirmations)},
        { typeof(PasswordResetConfirmationEntity),  new(Schemas.Auth, Tables.PasswordResetConfirmations)},
    };

    public static ConfirmationConfiguration Get<TConfirmationEntity>() where TConfirmationEntity : BaseConfirmationEntity<TConfirmationEntity>, new()
        => _configurations[typeof(TConfirmationEntity)];
}