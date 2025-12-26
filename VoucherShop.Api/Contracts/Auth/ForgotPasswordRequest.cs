namespace VoucherShop.Api.Contracts.Auth;

public record ForgotPasswordRequest(
    Guid ShopId,
    string Email
);
