using System.Text.RegularExpressions;

namespace Ilon.BuildingBlocks.Primitives;

/// <summary>
/// Represents an international phone number value object.
/// Validates phone numbers for Cameroon (+237) and other African countries.
/// </summary>
public sealed record PhoneNumber
{
    private static readonly Regex CameroonPattern = new(@"^\+237[0-9]{9}$", RegexOptions.Compiled);
    private static readonly Regex InternationalPattern = new(@"^\+[1-9][0-9]{7,14}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a PhoneNumber from a string value.
    /// </summary>
    public static PhoneNumber? Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = NormalizePhoneNumber(value);

        if (!IsValid(normalized))
            return null;

        return new PhoneNumber(normalized);
    }

    /// <summary>
    /// Validates if a phone number is valid.
    /// </summary>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = NormalizePhoneNumber(value);

        // Check Cameroon pattern first (most common for this app)
        if (CameroonPattern.IsMatch(normalized))
            return true;

        // Check general international pattern
        return InternationalPattern.IsMatch(normalized);
    }

    /// <summary>
    /// Normalizes a phone number by removing spaces, dashes, and other formatting.
    /// </summary>
    private static string NormalizePhoneNumber(string value)
    {
        return value.Replace(" ", "")
                   .Replace("-", "")
                   .Replace("(", "")
                   .Replace(")", "")
                   .Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
