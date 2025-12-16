using VoucherShop.Domain.Entities;

namespace VoucherShop.Application.Interfaces;

public interface IAuditRepository
{
    void Add(AuditLog audit);
}
