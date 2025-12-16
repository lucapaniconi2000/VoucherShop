namespace VoucherShop.Application.Vouchers.Admin.Dto;

public sealed record VoucherAuditDto(
    string Action,
    string ChangesJson,
    Guid? PerformedByUserId,
    DateTime PerformedAt
);