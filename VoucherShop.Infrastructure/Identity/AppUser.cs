using Microsoft.AspNetCore.Identity;

namespace VoucherShop.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public Guid ShopId { get; set; }
}
