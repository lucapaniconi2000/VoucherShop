using VoucherShop.Domain.Entities;

namespace VoucherShop.Application.Interfaces;

public interface IVoucherRepository
{
    Task<Voucher?> GetByShopAndUserIdAsync(Guid shopId, Guid userId, CancellationToken ct);
    void Add(Voucher voucher);
    Task SaveChangesAsync(CancellationToken ct);
}
