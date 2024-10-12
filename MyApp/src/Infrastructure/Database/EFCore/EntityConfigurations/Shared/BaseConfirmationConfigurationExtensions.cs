using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Shared;

public static class BaseConfirmationConfigurationExtensions
{
    public static void ConfigureConfirmation<TConfirmationEntity>(
        this EntityTypeBuilder<TConfirmationEntity> builder,
        Expression<Func<UserEntity, TConfirmationEntity?>> navigationPropertyGetter
        ) where TConfirmationEntity : BaseConfirmationEntity
    {
        var (schemaName, tableName) = ConfirmationConfigurations.Get<TConfirmationEntity>();
        builder.ToTable(tableName, schemaName);

        builder.HasKey(x => x.Id)
            .HasName($"pk_{tableName}_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(navigationPropertyGetter)
            .HasForeignKey<TConfirmationEntity>(x => x.UserId)
            .HasConstraintName($"fk_{tableName}_user_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName($"uq_{tableName}_user_id");

        builder.Property(x => x.Code)
            .HasColumnType($"CHAR({BaseConfirmationConstants.CodeLength})")
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName($"uq_{tableName}_code");

        builder.HasQueryFilter(x => x.User.IsEmailConfirmed == true);
    }
}
