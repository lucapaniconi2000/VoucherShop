using MediatR;
using VoucherShop.Application.Vouchers.Admin.Dto;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetVoucherAudit;

public sealed record GetVoucherAuditQuery(Guid UserId)
    : IRequest<IReadOnlyList<VoucherAuditDto>>;
