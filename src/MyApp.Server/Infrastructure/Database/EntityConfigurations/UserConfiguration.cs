using MyApp.Server.Domain.Auth.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static MyApp.Server.Infrastructure.Database.Constants;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(Tables.Users, Schemas.Auth);

        builder.HasKey(x => x.Id)
            .HasName("pk_users_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

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

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasQueryFilter(u => u.IsEmailConfirmed == true);
    }
}
