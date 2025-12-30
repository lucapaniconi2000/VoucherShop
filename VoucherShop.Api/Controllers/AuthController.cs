using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Api.Contracts.Auth;
using VoucherShop.Application.Interfaces;
using VoucherShop.Domain.Entities;
using VoucherShop.Infrastructure.Auth;
using VoucherShop.Infrastructure.Identity;
using VoucherShop.Infrastructure.Persistence;

namespace VoucherShop.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;
    private readonly ICurrentUser _currentUser;

    public AuthController(
        UserManager<AppUser> userManager,
        AppDbContext db,
        JwtTokenService jwt,
        ICurrentUser currentUser)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
        _currentUser = currentUser;
    }

    // ✅ BOOTSTRAP TENANT: crea Shop + Admin
    [HttpPost("register-shop")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterShop(
        [FromBody] RegisterShopRequest request,
        CancellationToken ct)
    {
        // email unica globale (semplificazione)
        var exists = await _userManager.FindByEmailAsync(request.Email);
        if (exists != null)
            return BadRequest("Admin already exists with this email.");

        // Crea Shop
        var shop = new Shop(request.ShopName, request.Currency);
        _db.Shops.Add(shop);

        // Crea Admin
        var admin = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            ShopId = shop.Id
        };

        var result = await _userManager.CreateAsync(admin, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(admin, "Admin");

        await _db.SaveChangesAsync(ct);

        return Ok(new RegisterShopResponse(shop.Id, admin.Id, admin.Email!));
    }

    // LOGIN
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        // ✅ token deve contenere shop_id (vedi JwtTokenService più sotto)
        var accessToken = _jwt.GenerateAccessToken(user, roles);

        var refreshToken = JwtTokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _db.SaveChangesAsync(ct);
        SetRefreshCookie(refreshToken);

        return Ok(new { accessToken });
    }

    // REFRESH
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken is null)
            return Unauthorized();

        var stored = await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r =>
                r.Token == refreshToken &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow,
                ct);

        if (stored is null)
            return Unauthorized();

        stored.IsRevoked = true;

        var roles = await _userManager.GetRolesAsync(stored.User);
        var newAccessToken = _jwt.GenerateAccessToken(stored.User, roles);
        var newRefreshToken = JwtTokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = stored.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _db.SaveChangesAsync(ct);

        SetRefreshCookie(newRefreshToken);

        return Ok(new { accessToken = newAccessToken });
    }

    // LOGOUT
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (refreshToken != null)
        {
            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken, ct);

            if (stored != null)
            {
                stored.IsRevoked = true;
                await _db.SaveChangesAsync(ct);
            }
        }

        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    // ✅ Registrazione self-service per utenti finali su uno shop specifico
    [AllowAnonymous]
    [HttpPost("self-register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SelfRegister(
        [FromBody] SelfRegisterRequest request,
        CancellationToken ct)
    {
        var shopExists = await _db.Shops.AnyAsync(s => s.Id == request.ShopId, ct);
        if (!shopExists)
            return NotFound("Shop not found.");

        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (existingUser != null)
        {
            var reason = existingUser.ShopId == request.ShopId
                ? "User already exists in this shop."
                : "User already exists in another shop.";

            return BadRequest(reason);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            ShopId = request.ShopId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return CreatedAtAction(
            nameof(SelfRegister),
            new SelfRegisterResponse(user.Id, user.Email!, user.ShopId)
        );
    }

    // ✅ Richiesta token reset password (email + ShopId per evitare enumerazione cross-tenant)
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.ShopId == request.ShopId, ct);

        if (user is null)
            return NotFound("User not found for this shop.");

        _ = await _userManager.GeneratePasswordResetTokenAsync(user);
        return NoContent();
    }

    // ✅ Reset password tenant-aware
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.ShopId == request.ShopId, ct);

        if (user is null)
            return NotFound("User not found for this shop.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var activeTokens = await _db.RefreshTokens
            .Where(r => r.UserId == user.Id && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
            token.IsRevoked = true;

        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ✅ Register tenant-aware (solo utenti autenticati, tipicamente Admin)
    // Crea un user nello stesso Shop del caller (shop_id da token)
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var shopId = _currentUser.ShopId;
        if (shopId == Guid.Empty)
            return Unauthorized("Missing shop_id claim.");

        var exists = await _userManager.FindByEmailAsync(request.Email);
        if (exists != null)
            return BadRequest("User already exists");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            ShopId = shopId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return CreatedAtAction(
            nameof(Register),
            new RegisterResponse(user.Id, user.Email!)
        );
    }

    private void SetRefreshCookie(string token)
    {
        Response.Cookies.Append(
            "refreshToken",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // Angular-friendly
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            });
    }
}
