using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoucherShop.Application.Vouchers.Queries.GetMyVoucher;

namespace VoucherShop.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public sealed class MeController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("voucher")]
    [ProducesResponseType(typeof(MyVoucherDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyVoucher(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyVoucherQuery(), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
