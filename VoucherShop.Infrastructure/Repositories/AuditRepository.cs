using VoucherShop.Application.Interfaces;
using VoucherShop.Domain.Entities;
using VoucherShop.Infrastructure.Persistence;

namespace VoucherShop.Infrastructure.Repositories;

public sealed class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _db;

    public AuditRepository(AppDbContext db)
    {
        _db = db;
    }

    public void Add(AuditLog audit)
    {
        _db.AuditLogs.Add(audit);
    }
}
