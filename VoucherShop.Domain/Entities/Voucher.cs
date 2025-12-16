using VoucherShop.Domain.ValueObjects;

namespace VoucherShop.Domain.Entities;

public sealed class Voucher
{
    public Guid Id { get; private set; }

    public Guid ShopId { get; private set; }   // ✅ Tenant
    public Guid UserId { get; private set; }   // utente dentro al tenant

    public Money Balance { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private Voucher() { } // EF

    public Voucher(Guid shopId, Guid userId, Money initialBalance)
    {
        if (shopId == Guid.Empty) throw new ArgumentException("ShopId cannot be empty.", nameof(shopId));
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        Id = Guid.NewGuid();
        ShopId = shopId;
        UserId = userId;

        Balance = initialBalance ?? throw new ArgumentNullException(nameof(initialBalance));

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        ExpiresAt = CreatedAt.AddMonths(3);
    }

    // ✅ Admin cambia solo l'importo (currency resta uguale)
    public void UpdateAmount(decimal newAmount)
    {
        Balance = Balance.WithAmount(newAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired(DateTime nowUtc) => nowUtc > ExpiresAt;
}
