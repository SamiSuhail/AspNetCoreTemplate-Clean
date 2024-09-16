using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Shared;
using static MyApp.Server.Infrastructure.Database.Constants;

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
        { typeof(EmailChangeConfirmationEntity),  new(Schemas.UserManagement, Tables.EmailChangeConfirmations)},
    };

    public static ConfirmationConfigurationValues Get<TConfirmationEntity>() where TConfirmationEntity : BaseConfirmationEntity
        => _configurations[typeof(TConfirmationEntity)];
}