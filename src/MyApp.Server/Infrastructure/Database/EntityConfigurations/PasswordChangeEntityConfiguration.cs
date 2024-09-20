using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Server.Infrastructure.Database.EntityConfigurations.Shared;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class PasswordChangeEntityConfiguration : IEntityTypeConfiguration<PasswordChangeConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<PasswordChangeConfirmationEntity> builder)
    {
        builder.ConfigureConfirmation(u => u.PasswordChangeConfirmation);

        builder.Property(e => e.NewPasswordHash)
            .HasMaxLength(UserConstants.PasswordHashLength)
            .IsRequired();
    }
}
