using Microsoft.EntityFrameworkCore;
using UserManagement.Domain;

namespace UserManagement.Infrastructure;

public sealed class UserDbContext : DbContext
{
    private readonly string _schema;
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        _schema = (options.FindExtension<
            Microsoft.EntityFrameworkCore.Infrastructure.RelationalOptionsExtension>()?
            .MigrationsHistoryTableSchema) ?? "user_management";
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<PermittedAction> PermittedActions => Set<PermittedAction>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserWarehouseProjection> UserWarehouseProjections => Set<UserWarehouseProjection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_schema);

        // User
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("app_user"); // avoid reserved word "user"
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.Phone).HasMaxLength(32);
            b.Property(x => x.MfaEnabled).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.PasswordHash);
            b.Property(x => x.PasswordCreatedAt);
            b.Property(x => x.PasswordLastChangedAt);
        });

        // Role
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("app_role"); // avoid reserved word "role"
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.HasIndex(x => x.Name).IsUnique();
            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.Description);
            b.Property(x => x.IsCommissionRole).IsRequired();
            b.Property(x => x.CommissionItemId);
        });

        // UserRole (many-to-many)
        modelBuilder.Entity<UserRole>(b =>
        {
            b.ToTable("user_role");
            b.HasKey(x => new { x.UserId, x.RoleId });
            b.Property(x => x.EffectiveFrom);

            b.HasOne(x => x.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Menu
        modelBuilder.Entity<Menu>(b =>
        {
            b.ToTable("menu");
            b.HasKey(x => x.Id);
            b.Property(x => x.Key).IsRequired().HasMaxLength(64);
            b.HasIndex(x => x.Key).IsUnique();
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.Route).IsRequired().HasMaxLength(256);
            b.Property(x => x.SortOrder).IsRequired();
            b.Property(x => x.ParentId);

            b.HasOne(x => x.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many Menu <-> PermittedAction
            b.HasMany(x => x.Actions)
                .WithMany()
                .UsingEntity(j => j.ToTable("menu_action"));
        });

        // PermittedAction
        modelBuilder.Entity<PermittedAction>(b =>
        {
            b.ToTable("permitted_action");
            b.HasKey(x => x.Id);
            b.Property(x => x.KeyEnum).IsRequired(); // enum stored as int by default
            b.HasAlternateKey(x => x.KeyEnum);
            b.HasIndex(x => x.KeyEnum).IsUnique();
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(64);
        });

        // Permission (Role ▸ Menu ▸ Action)
        modelBuilder.Entity<Permission>(b =>
        {
            b.ToTable("permission");
            b.HasKey(x => new { x.RoleId, x.MenuId, x.ActionKeyEnum });
            b.Property(x => x.Allowed).IsRequired();

            b.HasOne(x => x.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Menu)
                .WithMany()
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // Map ActionKeyEnum to PermittedAction via alternate key on KeyEnum
            b.HasOne(x => x.Action)
                .WithMany()
                .HasForeignKey(x => x.ActionKeyEnum)
                .HasPrincipalKey(a => a.KeyEnum)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UserWarehouseProjection (projection table)
        modelBuilder.Entity<UserWarehouseProjection>(b =>
        {
            b.ToTable("user_warehouse_projection");
            b.HasKey(x => new { x.UserId, x.WarehouseId });
            b.Property(x => x.IsDefault).IsRequired();
            b.Property(x => x.WarehouseCode).IsRequired().HasMaxLength(64);
            b.Property(x => x.WarehouseName).IsRequired().HasMaxLength(256);
            b.Property(x => x.Status).IsRequired().HasMaxLength(64);
            b.Property(x => x.OtherMetadata).HasColumnType("jsonb");
        });
    }
}
