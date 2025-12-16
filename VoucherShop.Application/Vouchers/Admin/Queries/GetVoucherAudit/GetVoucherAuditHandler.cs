using MediatR;
using VoucherShop.Application.Interfaces;
using VoucherShop.Application.Vouchers.Admin.Dto;

namespace VoucherShop.Application.Vouchers.Admin.Queries.GetVoucherAudit;

public sealed class GetVoucherAuditHandler
    : IRequestHandler<GetVoucherAuditQuery, IReadOnlyList<VoucherAuditDto>>
{
    private readonly IAuditReadRepository _audit;
    private readonly ICurrentUser _currenUser;

    public GetVoucherAuditHandler(
        IAuditReadRepository audit, 
        ICurrentUser currenUser)
    {
        _audit = audit;
        _currenUser = currenUser;
    }

    public async Task<IReadOnlyList<VoucherAuditDto>> Handle(GetVoucherAuditQuery request, CancellationToken cancellationToken)
    {
        var shopId = _currenUser.ShopId;

        if (shopId == Guid.Empty)
            throw new UnauthorizedAccessException("Missing shop context.");


        return await _audit.GetVoucherHistoryAsync(shopId, request.UserId, cancellationToken);
    }
}
