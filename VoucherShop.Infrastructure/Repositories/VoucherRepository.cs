using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;
using VoucherShop.Domain.Entities;
using VoucherShop.Infrastructure.Persistence;

namespace VoucherShop.Infrastructure.Repositories;

public sealed class VoucherRepository : IVoucherRepository
{
    private readonly AppDbContext _db;

    public VoucherRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Voucher?> GetByShopAndUserIdAsync(Guid shopId, Guid userId, CancellationToken ct)
        => _db.Vouchers.SingleOrDefaultAsync(v => v.ShopId == shopId && v.UserId == userId, ct);

    public void Add(Voucher voucher) => _db.Vouchers.Add(voucher);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}