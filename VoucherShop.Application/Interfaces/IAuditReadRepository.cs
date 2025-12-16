using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Vouchers.Admin.Dto;

namespace VoucherShop.Application.Interfaces;

public interface IAuditReadRepository
{
    Task<IReadOnlyList<VoucherAuditDto>> GetVoucherHistoryAsync(
        Guid shopId,
        Guid userId,
        CancellationToken ct);
}
