using MediatR;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetAllVouchers;

public sealed record GetAllVouchersQuery()
    : IRequest<IReadOnlyList<AdminVoucherDto>>;
