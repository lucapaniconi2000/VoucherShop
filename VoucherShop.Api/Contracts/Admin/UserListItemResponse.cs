namespace VoucherShop.Api.Contracts.Admin;

public sealed record UserListItemResponse(
    Guid Id,
    string Email,
    string? UserName
);
