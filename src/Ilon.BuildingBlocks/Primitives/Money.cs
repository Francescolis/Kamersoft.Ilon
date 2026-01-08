namespace Ilon.BuildingBlocks.Primitives;

/// <summary>
/// Represents a monetary value with currency.
/// </summary>
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a Money value object.
    /// </summary>
    public static Money? Create(decimal amount, string currency)
    {
        if (amount < 0)
            return null;

        if (string.IsNullOrWhiteSpace(currency))
            return null;

        var normalizedCurrency = currency.ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
            return null;

        return new Money(amount, normalizedCurrency);
    }

    /// <summary>
    /// Creates a Money value in XAF (Central African CFA Franc - Cameroon's currency).
    /// </summary>
    public static Money? InXAF(decimal amount) => Create(amount, "XAF");

    /// <summary>
    /// Creates a Money value in USD.
    /// </summary>
    public static Money? InUSD(decimal amount) => Create(amount, "USD");

    /// <summary>
    /// Creates a Money value in EUR.
    /// </summary>
    public static Money? InEUR(decimal amount) => Create(amount, "EUR");

    public override string ToString() => $"{Amount:N2} {Currency}";

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }
}
