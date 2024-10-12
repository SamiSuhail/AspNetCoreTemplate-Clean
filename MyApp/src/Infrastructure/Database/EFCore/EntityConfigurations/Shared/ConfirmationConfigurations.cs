﻿using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using static MyApp.Infrastructure.Database.Constants;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Shared;

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