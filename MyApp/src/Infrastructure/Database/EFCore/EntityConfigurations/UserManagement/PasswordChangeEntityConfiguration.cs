using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Auth.User;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Shared;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.UserManagement;

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
