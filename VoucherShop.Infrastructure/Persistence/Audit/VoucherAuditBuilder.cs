using VoucherShop.Domain.Entities;

namespace VoucherShop.Infrastructure.Persistence.Audit;

public static class VoucherAuditBuilder
{
    public static Dictionary<string, AuditChange> Build(
        Voucher voucher,
        IDictionary<string, object?> originalValues)
    {
        return new Dictionary<string, AuditChange>
        {
            ["amount"] = new AuditChange
            {
                Old = originalValues["Balance.Amount"],
                New = voucher.Balance.Amount
            },
            ["currency"] = new AuditChange
            {
                Old = originalValues["Balance.Currency"],
                New = voucher.Balance.Currency
            },
            ["expiresAt"] = new AuditChange
            {
                Old = originalValues["ExpiresAt"],
                New = voucher.ExpiresAt
            }
        };
    }
}
