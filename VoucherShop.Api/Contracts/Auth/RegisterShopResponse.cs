namespace VoucherShop.Api.Contracts.Auth;

public sealed record RegisterShopResponse(
    Guid ShopId,
    Guid AdminUserId,
    string AdminEmail
);
