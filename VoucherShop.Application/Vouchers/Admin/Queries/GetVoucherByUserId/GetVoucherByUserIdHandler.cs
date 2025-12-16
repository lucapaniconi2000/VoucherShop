using MediatR;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetVoucherByUserId;

public sealed class GetVoucherByUserIdHandler 
    : IRequestHandler<GetVoucherByUserIdQuery, AdminVoucherDto?>
{
    private readonly IVoucherReadContext _db;

    public GetVoucherByUserIdHandler(IVoucherReadContext db)
    {
        _db = db;
    }

    public async Task<AdminVoucherDto?> Handle(
        GetVoucherByUserIdQuery request,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await _db.Vouchers
            .AsNoTracking()
            .Where(v => v.UserId == request.UserId)
            .Select(v => new AdminVoucherDto(
                v.UserId,
                v.Balance.Amount,
                v.Balance.Currency,
                v.UpdatedAt,
                v.ExpiresAt,
                v.ExpiresAt < now
            ))
            .SingleOrDefaultAsync(ct);
    }
}