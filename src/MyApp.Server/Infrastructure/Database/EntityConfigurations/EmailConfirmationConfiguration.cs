using MyApp.Server.Domain.Auth.EmailConfirmation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static MyApp.Server.Infrastructure.Database.Constants;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class EmailConfirmationConfiguration : IEntityTypeConfiguration<EmailConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationEntity> builder)
    {
        builder.ToTable(Tables.EmailConfirmations, Schemas.Auth);

        builder.HasKey(x => x.Id)
            .HasName("pk_email_confirmations_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.EmailConfirmation)
            .HasForeignKey<EmailConfirmationEntity>(x => x.UserId)
            .HasConstraintName("fk_email_confirmations_user_id");

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("uq_email_confirmations_user_id");

        builder.Property(x => x.Code)
            .HasColumnType($"CHAR({EmailConfirmationConstants.CodeLength})")
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("uq_email_confirmations_code");

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
