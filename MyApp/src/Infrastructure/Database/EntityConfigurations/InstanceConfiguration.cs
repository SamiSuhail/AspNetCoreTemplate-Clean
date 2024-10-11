using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Infra.Instance;
using static MyApp.Infrastructure.Database.Constants;

namespace MyApp.Infrastructure.Database.EntityConfigurations;

public class InstanceConfiguration : IEntityTypeConfiguration<InstanceEntity>
{
    public void Configure(EntityTypeBuilder<InstanceEntity> builder)
    {
        builder.ToTable(Tables.Instances, Schemas.Infra);

        builder.HasKey(x => x.Id)
            .HasName("pk_instances_id");
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasMaxLength(InstanceConstants.NameMaxLength)
            .IsRequired();
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("uq_instances_name");

        builder.Property(x => x.IsCleanupEnabled)
            .IsRequired();
    }
}
