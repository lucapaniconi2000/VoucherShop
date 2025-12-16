namespace VoucherShop.Infrastructure.Persistence.Audit;

public sealed class AuditChange
{
    public object? Old { get; init; }
    public object? New { get; init; }
}
