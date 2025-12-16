using MediatR;

namespace VoucherShop.Application.Vouchers.Queries.GetMyVoucher;

public sealed record GetMyVoucherQuery() : IRequest<MyVoucherDto?>;

