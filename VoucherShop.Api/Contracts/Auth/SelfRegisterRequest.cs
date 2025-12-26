namespace VoucherShop.Api.Contracts.Auth;

public record SelfRegisterRequest(
    Guid ShopId,
    string Email,
    string Password
);
