using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;
using VoucherShop.Domain.Entities;
using VoucherShop.Infrastructure.Persistence;

namespace VoucherShop.Infrastructure.Repositories;

public sealed class ShopRepository : IShopRepository
{
    private readonly AppDbContext _db;

    public ShopRepository(AppDbContext db) => _db = db;

    public Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken ct)
        => _db.Shops.AsNoTracking().SingleOrDefaultAsync(s => s.Id == shopId, ct);
}
