namespace VoucherShop.Application.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid ShopId { get; }
    bool IsAuthenticated { get; }
}
