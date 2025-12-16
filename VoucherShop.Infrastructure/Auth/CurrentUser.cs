using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VoucherShop.Application.Interfaces;

namespace VoucherShop.Infrastructure;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public bool IsAuthenticated =>
        _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            if (!IsAuthenticated) return Guid.Empty;

            var raw = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }
    }

    public Guid ShopId
    {
        get
        {
            if (!IsAuthenticated) return Guid.Empty;

            var raw = _http.HttpContext!.User.FindFirstValue("shop_id");
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }
    }
}
