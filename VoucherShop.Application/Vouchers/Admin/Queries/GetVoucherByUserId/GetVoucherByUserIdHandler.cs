using MediatR;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetVoucherByUserId;

public sealed class GetVoucherByUserIdHandler 
    : IRequestHandler<GetVoucherByUserIdQuery, AdminVoucherDto?>
{
    private readonly IVoucherReadContext _db;
    private readonly ICurrentUser _currentUser;

    public GetVoucherByUserIdHandler(
        IVoucherReadContext db, 
        ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<AdminVoucherDto?> Handle(
        GetVoucherByUserIdQuery request,
        CancellationToken ct)
    {
        var shopId = _currentUser.ShopId;

        if (shopId == Guid.Empty)
            throw new UnauthorizedAccessException("Missing shop context.");

        var now = DateTime.UtcNow;

        return await _db.Vouchers
            .AsNoTracking()
            .Where(v => v.UserId == request.UserId)
            .Where(v => v.ShopId == shopId)
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
