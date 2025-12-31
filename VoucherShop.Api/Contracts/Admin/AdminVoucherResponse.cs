namespace VoucherShop.Api.Contracts.Admin;

public sealed record AdminVoucherResponse(
    Guid UserId,
    decimal Amount,
    string Currency,
    DateTime? ExpiresAtUtc,
    DateTime UpdateAtUtc
);
