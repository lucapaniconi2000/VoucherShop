public sealed record AdminVoucherDto(
    Guid UserId,
    decimal Amount,
    string Currency,
    DateTime UpdatedAt,
    DateTime ExpiresAt,
    bool IsExpired
);
