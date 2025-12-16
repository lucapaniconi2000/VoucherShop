using MediatR;

namespace VoucherShop.Application.Vouchers.Commands.UpdateVoucherAmount;

public sealed record UpdateVoucherAmountCommand(
    Guid TargetUserId,
    decimal NewAmount
) : IRequest;
