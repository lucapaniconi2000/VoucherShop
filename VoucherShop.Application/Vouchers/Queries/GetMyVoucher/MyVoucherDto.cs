using System.Linq.Expressions;
using VoucherShop.Domain.Entities;
using VoucherShop.Domain.Enums;

namespace VoucherShop.Application.Vouchers.Queries.GetMyVoucher;

public sealed record MyVoucherDto(
    decimal Amount,
    string Currency,
    DateTime UpdateAt,
    DateTime ExpiresAt,
    VoucherStatus Status
)
{
    public static Expression<Func<Voucher, MyVoucherDto>> Projection(DateTime now)
        => v => new MyVoucherDto(
            v.Balance.Amount,
            v.Balance.Currency,
            v.UpdatedAt,
            v.ExpiresAt,
            v.ExpiresAt < now ? VoucherStatus.Expired : VoucherStatus.Active
        );
}
