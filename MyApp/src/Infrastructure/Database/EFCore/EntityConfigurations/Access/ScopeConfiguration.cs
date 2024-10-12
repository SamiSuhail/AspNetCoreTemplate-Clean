using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Access.Scope;
using static MyApp.Infrastructure.Database.Constants;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Access;

public class ScopeConfiguration : IEntityTypeConfiguration<ScopeEntity>
{
    public void Configure(EntityTypeBuilder<ScopeEntity> builder)
    {
        builder.ToTable(Tables.Scopes, Schemas.Access);

        builder.HasKey(x => x.Id)
            .HasName("pk_scopes_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasMaxLength(ScopeConstants.NameMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("uq_scopes_name");
    }
}
