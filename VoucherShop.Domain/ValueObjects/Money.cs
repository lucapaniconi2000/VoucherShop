namespace VoucherShop.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        currency = currency.Trim().ToUpperInvariant();

        if (currency.Length != 3 || !currency.All(char.IsLetter))
            throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    // 🟢 Value Object equality
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
        => Equals(obj as Money);

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);

    public static bool operator ==(Money left, Money right)
        => Equals(left, right);

    public static bool operator !=(Money left, Money right)
        => !Equals(left, right);

    // 🟢 Comodo se vuoi cambiare solo amount
    public Money WithAmount(decimal newAmount)
        => new(newAmount, Currency);

    // 🟢 Comodo se vuoi cambiare valuta
    public Money WithCurrency(string newCurrency)
        => new(Amount, newCurrency);
}
