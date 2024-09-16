using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using static MyApp.Server.Infrastructure.Database.Constants;
using MyApp.Server.Domain.Shared.Confirmations;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations.Shared;

public record ConfirmationConfigurationValues(
    string SchemaName,
    string TableName);

public static class ConfirmationConfigurations
{
    private static readonly Dictionary<Type, ConfirmationConfigurationValues> _configurations = new()
    {
        { typeof(UserConfirmationEntity),  new(Schemas.Auth, Tables.UserConfirmations)},
        { typeof(PasswordResetConfirmationEntity),  new(Schemas.Auth, Tables.PasswordResetConfirmations)},
    };

    public static ConfirmationConfigurationValues Get<TConfirmationEntity>() where TConfirmationEntity : BaseConfirmationEntity
        => _configurations[typeof(TConfirmationEntity)];
}