using MyApp.Server.Domain.Auth.EmailConfirmation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static MyApp.Server.Infrastructure.Database.Constants;
using MyApp.Server.Infrastructure.Database.EntityConfigurations.Shared;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class EmailConfirmationConfiguration : IEntityTypeConfiguration<EmailConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationEntity> builder)
    {
        builder.ConfigureConfirmation(u => u.EmailConfirmation);
    }
}
