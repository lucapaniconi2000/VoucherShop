namespace VoucherShop.Api.Contracts.Admin;

public sealed record UpdateVoucherRequest(
    decimal NewAmount,
    DateTime? NewExpiresAtUtc
);
