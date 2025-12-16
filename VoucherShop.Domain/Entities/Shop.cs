namespace VoucherShop.Domain.Entities;

public sealed class Shop
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Currency { get; private set; } = default!; // ISO 4217, es: EUR
    public DateTime CreatedAt { get; private set; }

    private Shop() { } // EF

    public Shop(string name, string currency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Shop name is required.", nameof(name));

        currency = (currency ?? "").Trim().ToUpperInvariant();
        if (currency.Length != 3 || !currency.All(char.IsLetter))
            throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Currency = currency;
        CreatedAt = DateTime.UtcNow;
    }
}
