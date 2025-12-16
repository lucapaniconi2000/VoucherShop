using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;
using VoucherShop.Application.Vouchers.Admin.Dto;

namespace VoucherShop.Infrastructure.Persistence.Audit;

public sealed class AuditReadRepository : IAuditReadRepository
{
    private readonly AppDbContext _db;

    public AuditReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<VoucherAuditDto>> GetVoucherHistoryAsync(
        Guid shopId,
        Guid userId,
        CancellationToken ct)
    {
        return await _db.AuditLogs
            .AsNoTracking()
            .Where(a =>
                a.ShopId == shopId &&
                a.EntityName == "Voucher" &&
                a.EntityId == userId)
            .OrderByDescending(a => a.PerformedAt)
            .Select(a => new VoucherAuditDto(
                a.Action,
                a.Changes,
                a.PerformedByUserId,
                a.PerformedAt
            ))
            .ToListAsync(ct);
    }
}
