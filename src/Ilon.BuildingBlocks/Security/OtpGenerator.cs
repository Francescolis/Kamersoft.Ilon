using System.Security.Cryptography;

namespace Ilon.BuildingBlocks.Security;

/// <summary>
/// Provides helpers for generating and validating OTPs (One-Time Passwords).
/// </summary>
public static class OtpGenerator
{
    /// <summary>
    /// Generates a random 6-digit OTP.
    /// </summary>
    public static string Generate(int length = 6)
    {
        if (length < 4 || length > 10)
            throw new ArgumentException("OTP length must be between 4 and 10 digits", nameof(length));

        var number = RandomNumberGenerator.GetInt32(
            (int)Math.Pow(10, length - 1),
            (int)Math.Pow(10, length)
        );

        return number.ToString($"D{length}");
    }

    /// <summary>
    /// Validates if a string is a valid OTP format.
    /// </summary>
    public static bool IsValidFormat(string otp, int expectedLength = 6)
    {
        if (string.IsNullOrWhiteSpace(otp))
            return false;

        if (otp.Length != expectedLength)
            return false;

        return otp.All(char.IsDigit);
    }
}
