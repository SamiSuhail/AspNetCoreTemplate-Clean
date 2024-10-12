using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Shared;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Auth;

public class PasswordResetConfirmationConfiguration : IEntityTypeConfiguration<PasswordResetConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<PasswordResetConfirmationEntity> builder)
    {
        builder.ConfigureConfirmation(u => u.PasswordResetConfirmation);
    }
}
