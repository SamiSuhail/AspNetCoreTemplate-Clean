using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static MyApp.Server.Infrastructure.Database.Constants;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class PasswordResetConfirmationConfiguration : IEntityTypeConfiguration<PasswordResetConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<PasswordResetConfirmationEntity> builder)
    {
        builder.ToTable(Tables.PasswordResetConfirmations, Schemas.Auth);

        builder.HasKey(x => x.Id)
            .HasName("pk_password_reset_confirmations_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.PasswordResetConfirmation)
            .HasForeignKey<PasswordResetConfirmationEntity>(x => x.UserId)
            .HasConstraintName("fk_password_reset_confirmations_user_id");

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("uq_password_reset_confirmations_user_id");

        builder.Property(x => x.Code)
            .HasColumnType($"CHAR({PasswordResetConfirmationConstants.CodeLength})")
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("uq_password_reset_confirmations_code");

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
