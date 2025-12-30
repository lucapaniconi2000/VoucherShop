using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoucherShop.Api.Contracts.Admin;
using VoucherShop.Application.Interfaces;
using VoucherShop.Infrastructure.Identity;

namespace VoucherShop.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public AdminUsersController(UserManager<AppUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest body)
    {
        var shopId = _currentUser.ShopId;
        if (shopId == Guid.Empty)
            return Unauthorized("Missing shop_id claim.");

        var exists = await _userManager.FindByEmailAsync(body.Email);
        if (exists != null)
            return BadRequest("User already exists");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = body.Email,
            UserName = body.Email,
            ShopId = shopId
        };

        var created = await _userManager.CreateAsync(user, body.Password);
        if (!created.Succeeded)
            return BadRequest(created.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { userId = user.Id });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserListItemResponse>>> GetUsers(CancellationToken ct)
    {
        var shopId = _currentUser.ShopId;
        if (shopId == Guid.Empty)
            throw new UnauthorizedAccessException();

        var list = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.ShopId == shopId)
            .OrderBy(u => u.Email)
            .Select(u => new UserListItemResponse(u.Id, u.Email!, u.UserName))
            .ToListAsync(ct);
        
        return Ok(list);
    }

}
