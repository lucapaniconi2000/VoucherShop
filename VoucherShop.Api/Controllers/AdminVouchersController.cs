using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoucherShop.Api.Contracts.Admin;
using VoucherShop.Application.Vouchers.Admin.Queries.GetVoucherAudit;
using VoucherShop.Application.Vouchers.Commands.UpdateVoucherAmount;

namespace VoucherShop.Api.Controllers;

[ApiController]
[Route("api/admin/vouchers")]
[Authorize(Roles = "Admin")]
public sealed class AdminVouchersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminVouchersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVoucher(
        Guid userId,
        [FromBody] UpdateVoucherRequest body,
        CancellationToken ct)
    {
        await _mediator.Send(
            new UpdateVoucherAmountCommand(TargetUserId: userId, NewAmount: body.NewAmount),
            ct);

        return NoContent();
    }

    [HttpGet("{userId:guid}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetHistory(
        Guid userId,
        CancellationToken ct)
    {
        var history = await _mediator.Send(new GetVoucherAuditQuery(userId), ct);
        return Ok(history);
    }
}
