namespace VoucherShop.Api.Contracts.Auth;

public sealed record RegisterShopRequest(
    string ShopName,
    string Currency,
    string Email, 
    string Password
);
