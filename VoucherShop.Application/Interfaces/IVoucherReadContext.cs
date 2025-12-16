using Microsoft.EntityFrameworkCore;
using VoucherShop.Domain.Entities;

namespace VoucherShop.Application.Interfaces;

public interface IVoucherReadContext
{
    DbSet<Voucher> Vouchers { get; }
}
