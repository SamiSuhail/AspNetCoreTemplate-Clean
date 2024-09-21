using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Shared;

namespace MyApp.Server.Infrastructure.Database;

public static class CustomModelBuilderExtensions
{
    public static void ApplyCustomConfigurations(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .ToArray();

        var entityClrTypes = entityTypes
            .Select(e => e.ClrType)
            .ToArray();

        ConfigureCreationAuditedEntities(modelBuilder, entityClrTypes);
    }

    private static void ConfigureCreationAuditedEntities(ModelBuilder modelBuilder, Type[] entityTypes)
    {
        var creationAuditedEntities = entityTypes
                    .Where(et => et != null && typeof(ICreationAudited).IsAssignableFrom(et));

        foreach (var creationAuditedEntity in creationAuditedEntities)
        {
            var method = GetMethod(nameof(ConfigureCreationAuditedEntitiesGeneric))?.MakeGenericMethod(creationAuditedEntity);
            method?.Invoke(null, [modelBuilder]);
        }

        static void ConfigureCreationAuditedEntitiesGeneric<TEntity>(ModelBuilder builder)
        where TEntity : class, ICreationAudited
        {
            builder.Entity<TEntity>()
                .Property(t => t.CreatedAt)
                .IsRequired();
        }
    }

    private static MethodInfo? GetMethod(string name)
        => typeof(CustomModelBuilderExtensions).GetMethod(
            name,
            BindingFlags.NonPublic | BindingFlags.Static);
}
