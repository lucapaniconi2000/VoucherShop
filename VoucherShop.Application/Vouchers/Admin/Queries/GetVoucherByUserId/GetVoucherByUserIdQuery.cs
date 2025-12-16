using MediatR;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetVoucherByUserId;

public sealed record GetVoucherByUserIdQuery(Guid UserId) 
    : IRequest<AdminVoucherDto?>;
