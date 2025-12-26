namespace VoucherShop.Api.Contracts.Auth;

public record SelfRegisterResponse(
    Guid UserId,
    string Email,
    Guid ShopId
);
