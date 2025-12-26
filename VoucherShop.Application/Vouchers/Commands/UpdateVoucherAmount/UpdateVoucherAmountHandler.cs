namespace VoucherShop.Application.Vouchers.Commands.UpdateVoucherAmount;

using System.Collections.Generic;
using System.Text.Json;
using MediatR;
using VoucherShop.Application.Common.Exceptions;
using VoucherShop.Application.Interfaces;
using VoucherShop.Domain.Entities;
using VoucherShop.Domain.ValueObjects;

public sealed class UpdateVoucherAmountHandler : IRequestHandler<UpdateVoucherAmountCommand>
{
    private readonly IVoucherRepository _voucherRepo;
    private readonly IShopRepository _shopRepo;
    private readonly IAuditRepository _auditRepo;
    private readonly ICurrentUser _currentUser;

    public UpdateVoucherAmountHandler(
        IVoucherRepository voucherRepo,
        IShopRepository shopRepo,
        IAuditRepository auditRepo,
        ICurrentUser currentUser)
    {
        _voucherRepo = voucherRepo;
        _shopRepo = shopRepo;
        _auditRepo = auditRepo;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateVoucherAmountCommand request, CancellationToken ct)
    {
        var shopId = _currentUser.ShopId;
        if (shopId == Guid.Empty)
            throw new UnauthorizedAccessException("Missing shop context.");

        var shop = await _shopRepo.GetByIdAsync(shopId, ct)
            ?? throw new NotFoundException("Shop not found.");

        var voucher = await _voucherRepo.GetByShopAndUserIdAsync(shopId, request.TargetUserId, ct);
        var isNewVoucher = voucher is null;
        var now = DateTime.UtcNow;

        // Snapshot per audit
        decimal? oldAmount = voucher?.Balance.Amount;
        string? oldCurrency = voucher?.Balance.Currency;
        DateTime? oldExpiresAt = voucher?.ExpiresAt;

        if (request.NewExpiresAtUtc is { } requestedExpires && requestedExpires <= now)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.NewExpiresAtUtc)] = new[] { "Expiration must be in the future." }
            });
        }

        if (isNewVoucher)
        {
            // ✅ create (insert)
            var initial = new Money(request.NewAmount, shop.Currency);
            voucher = new Voucher(shopId, request.TargetUserId, initial);
            _voucherRepo.Add(voucher);

            if (request.NewExpiresAtUtc is { } newExpires)
            {
                voucher.OverrideExpiration(newExpires);
            }
        }
        else
        {
            // ✅ update (solo amount)
            voucher.UpdateAmount(request.NewAmount);

            if (request.NewExpiresAtUtc is { } overrideExpiresAt)
            {
                voucher.OverrideExpiration(overrideExpiresAt);
            }
            else if (oldAmount is not null && request.NewAmount > oldAmount)
            {
                voucher.RenewExpiration(now);
            }
        }

        // ✅ audit business
        _auditRepo.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            ShopId = shopId,
            EntityName = nameof(Voucher),
            EntityId = voucher.UserId, // chiave logica per "history" di quell'utente
            Action = isNewVoucher ? "Create" : "Update", // se vuoi, calcolalo prima
            Changes = JsonSerializer.Serialize(new
            {
                amount = new { old = oldAmount, @new = voucher.Balance.Amount },
                currency = new { old = oldCurrency, @new = voucher.Balance.Currency },
                expiresAt = new { old = oldExpiresAt, @new = voucher.ExpiresAt }
            }),
            PerformedByUserId = _currentUser.UserId,
            PerformedAt = DateTime.UtcNow
        });

        await _voucherRepo.SaveChangesAsync(ct);
    }
}
