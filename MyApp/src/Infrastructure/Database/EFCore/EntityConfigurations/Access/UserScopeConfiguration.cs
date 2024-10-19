using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Access.Scope;
using static MyApp.Infrastructure.Database.Constants;

namespace MyApp.Infrastructure.Database.EFCore.EntityConfigurations.Access;

public class UserScopeConfiguration : IEntityTypeConfiguration<UserScopeEntity>
{
    public void Configure(EntityTypeBuilder<UserScopeEntity> builder)
    {
        builder.ToTable(Tables.UserScopes, Schemas.Access);

        builder.HasKey(x => new { x.UserId, x.ScopeId })
            .HasName("pk_user_scopes_user_id_scope_id");

        builder.Property(x => x.UserId)
            .IsRequired();
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserScopes)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ScopeId)
            .IsRequired();
        builder.HasOne(x => x.Scope)
            .WithMany(x => x.ScopeUsers)
            .HasForeignKey(x => x.ScopeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => x.User.IsEmailConfirmed == true);
    }
}
