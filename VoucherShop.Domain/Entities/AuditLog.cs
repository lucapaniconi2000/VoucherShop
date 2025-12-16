namespace VoucherShop.Domain.Entities;

public sealed class AuditLog
{
    public Guid Id { get; set; }

    public Guid ShopId { get; set; }

    public string EntityName { get; set; } = default!;
    public Guid EntityId { get; set; } // nel tuo caso: UserId del voucher
    public string Action { get; set; } = default!;
    public string Changes { get; set; } = default!;

    public Guid? PerformedByUserId { get; set; }
    public DateTime PerformedAt { get; set; }
}
