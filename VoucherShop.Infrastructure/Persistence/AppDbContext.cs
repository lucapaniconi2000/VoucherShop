using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;
using VoucherShop.Domain.Entities;
using VoucherShop.Infrastructure.Identity;

namespace VoucherShop.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>,
      IVoucherReadContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shop>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.Property(x => x.ShopId).IsRequired();
            e.HasIndex(x => new { x.ShopId, x.Email }).IsUnique(false);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.ShopId).IsRequired();
            entity.Property(v => v.UserId).IsRequired();

            // ✅ 1 voucher per utente per negozio
            entity.HasIndex(v => new { v.ShopId, v.UserId }).IsUnique();

            entity.Property(v => v.CreatedAt).IsRequired();
            entity.Property(v => v.UpdatedAt).IsRequired();
            entity.Property(v => v.ExpiresAt).IsRequired();

            entity.OwnsOne(v => v.Balance, money =>
            {
                money.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.ShopId).IsRequired();

            entity.Property(a => a.EntityName).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Action).HasMaxLength(20).IsRequired();

            entity.Property(a => a.Changes).IsRequired();
            entity.Property(a => a.PerformedAt).IsRequired();

            // ✅ indice per query veloci su history
            entity.HasIndex(a => new { a.ShopId, a.EntityName, a.EntityId });
        });

    }
}
