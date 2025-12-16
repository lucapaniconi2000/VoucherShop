namespace VoucherShop.Api.Contracts.Auth;

public sealed record RegisterResponse(
    Guid UserId,
    string Email
);
