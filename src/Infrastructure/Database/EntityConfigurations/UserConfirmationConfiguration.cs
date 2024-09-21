using MyApp.Domain.Auth.UserConfirmation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Infrastructure.Database.EntityConfigurations.Shared;

namespace MyApp.Infrastructure.Database.EntityConfigurations;

public class UserConfirmationConfiguration : IEntityTypeConfiguration<UserConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<UserConfirmationEntity> builder)
    {
        builder.ConfigureConfirmation(u => u.UserConfirmation);
    }
}
