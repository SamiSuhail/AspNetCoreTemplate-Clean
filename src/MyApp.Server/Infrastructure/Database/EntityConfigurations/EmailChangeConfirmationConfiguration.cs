using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Shared.Confirmations;
using static MyApp.Server.Infrastructure.Database.Constants;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class EmailChangeConfirmationConfiguration : IEntityTypeConfiguration<EmailChangeConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<EmailChangeConfirmationEntity> builder)
    {
        builder.ToTable(Tables.EmailChangeConfirmations, Schemas.UserManagement);

        builder.HasKey(x => x.Id)
            .HasName("pk_email_change_confirmations_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.EmailChangeConfirmation)
            .HasForeignKey<EmailChangeConfirmationEntity>(x => x.UserId)
            .HasConstraintName("fk_email_change_confirmations_user_id");

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("uq_email_change_confirmations_user_id");

        builder.Property(x => x.OldEmailCode)
            .HasColumnType($"CHAR({BaseConfirmationConstants.CodeLength})")
            .IsRequired();

        builder.HasIndex(x => x.OldEmailCode)
            .IsUnique()
            .HasDatabaseName("uq_email_change_confirmations_old_email_code");

        builder.Property(x => x.NewEmailCode)
            .HasColumnType($"CHAR({BaseConfirmationConstants.CodeLength})")
            .IsRequired();

        builder.HasIndex(x => x.NewEmailCode)
            .IsUnique()
            .HasDatabaseName("uq_email_change_confirmations_new_email_code");

        builder.Property(x => x.NewEmail)
            .HasMaxLength(UserConstants.EmailMaxLength)
            .IsRequired();
    }
}
