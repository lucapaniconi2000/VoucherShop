namespace VoucherShop.Api.Contracts.Admin;

public sealed record CreateUserResponse(
    Guid UserId,
    string Email
);

