using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using static MyApp.Server.Infrastructure.Database.Constants;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation;

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
        { typeof(PasswordChangeConfirmationEntity),  new(Schemas.UserManagement, Tables.PasswordChangeConfirmations)},
    };

    public static ConfirmationConfigurationValues Get<TConfirmationEntity>() where TConfirmationEntity : BaseConfirmationEntity
        => _configurations[typeof(TConfirmationEntity)];
}