using MediatR;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Application.Interfaces;

namespace VoucherShop.Application.Vouchers.Queries.GetMyVoucher;

public sealed class GetMyVoucherHandler
    : IRequestHandler<GetMyVoucherQuery, MyVoucherDto?>
{
    private readonly IVoucherReadContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMyVoucherHandler(
        IVoucherReadContext db,
        ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<MyVoucherDto?> Handle(
        GetMyVoucherQuery request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var shopId = _currentUser.ShopId;

        // opzionale ma consigliato: se non autenticato, non cercare nulla
        if (userId == Guid.Empty || shopId == Guid.Empty)
            return null;

        var now = DateTime.UtcNow;

        return await _db.Vouchers
            .AsNoTracking()
            .Where(v => v.ShopId == shopId && v.UserId == userId)
            .Select(MyVoucherDto.Projection(now))
            .SingleOrDefaultAsync(ct);
    }
}
