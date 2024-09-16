using MyApp.Server.Domain.Auth.UserConfirmation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Server.Infrastructure.Database.EntityConfigurations.Shared;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;

namespace MyApp.Server.Infrastructure.Database.EntityConfigurations;

public class EmailChangeConfirmationConfiguration : IEntityTypeConfiguration<EmailChangeConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<EmailChangeConfirmationEntity> builder)
    {
        builder.ConfigureConfirmation(u => u.EmailChangeConfirmation);
    }
}
