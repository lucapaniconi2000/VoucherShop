using VoucherShop.Domain.Entities;

namespace VoucherShop.Application.Interfaces;

public interface IShopRepository
{
    Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken ct);
}
