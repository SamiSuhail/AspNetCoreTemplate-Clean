using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Infrastructure.Database.EntityConfigurations.Shared;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class PasswordResetConfirmationConfiguration : IEntityTypeConfiguration<PasswordResetConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<PasswordResetConfirmationEntity> builder)
    {
        builder.ConfigureConfirmation(u => u.PasswordResetConfirmation);
    }
}
