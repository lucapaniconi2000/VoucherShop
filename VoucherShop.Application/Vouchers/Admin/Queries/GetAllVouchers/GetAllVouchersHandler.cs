using MediatR;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetAllVouchers;

public sealed class GetAllVouchersHandler
    : IRequestHandler<GetAllVouchersQuery, IReadOnlyList<AdminVoucherDto>>
{
    private readonly IVoucherReadContext _db;

    public GetAllVouchersHandler(IVoucherReadContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<AdminVoucherDto>> Handle(
        GetAllVouchersQuery request,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await _db.Vouchers
            .AsNoTracking()
            .Select(v => new AdminVoucherDto(
                v.UserId,
                v.Balance.Amount,
                v.Balance.Currency,
                v.UpdatedAt,
                v.ExpiresAt,
                v.ExpiresAt < now
            ))
            .ToListAsync(ct);
    }
}