using MyApp.Domain.Auth.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static MyApp.Infrastructure.Database.Constants;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Auth;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(Tables.Users, Schemas.Auth);

        builder.HasKey(x => x.Id)
            .HasName("pk_users_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.InstanceId)
            .IsRequired();
        builder.HasOne(x => x.Instance)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.InstanceId)
            .HasConstraintName("fk_users_instance_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Username)
            .HasMaxLength(UserConstants.UsernameMaxLength)
            .IsRequired();
        builder.HasIndex(x => x.Username)
            .IsUnique()
            .HasDatabaseName("uq_users_username");

        builder.Property(x => x.PasswordHash)
            .HasColumnType($"CHAR({UserConstants.PasswordHashLength})")
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(UserConstants.EmailMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("uq_users_email");

        builder.Property(x => x.IsEmailConfirmed)
            .IsRequired();
        builder.HasIndex(x => x.IsEmailConfirmed);
        builder.HasQueryFilter(u => u.IsEmailConfirmed == true);

        builder.Property(x => x.RefreshTokenVersion)
            .IsRequired();

    }
}
