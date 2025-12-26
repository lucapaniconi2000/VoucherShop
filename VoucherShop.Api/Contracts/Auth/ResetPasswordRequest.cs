namespace VoucherShop.Api.Contracts.Auth;

public record ResetPasswordRequest(
    Guid ShopId,
    string Email,
    string Token,
    string NewPassword
);
