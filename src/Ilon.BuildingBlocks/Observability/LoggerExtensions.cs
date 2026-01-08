using Microsoft.Extensions.Logging;

namespace Ilon.BuildingBlocks.Observability;

/// <summary>
/// Provides extension methods for structured logging.
/// </summary>
public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> _logOtpSent =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1001, "OtpSent"),
            "OTP sent to phone number {PhoneNumber}");

    private static readonly Action<ILogger, string, Exception?> _logOtpVerified =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1002, "OtpVerified"),
            "OTP verified for phone number {PhoneNumber}");

    private static readonly Action<ILogger, string, string, Exception?> _logValidationFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            new EventId(2001, "ValidationFailed"),
            "Validation failed for {RequestType}: {Errors}");

    public static void LogOtpSent(this ILogger logger, string phoneNumber)
        => _logOtpSent(logger, MaskPhoneNumber(phoneNumber), null);

    public static void LogOtpVerified(this ILogger logger, string phoneNumber)
        => _logOtpVerified(logger, MaskPhoneNumber(phoneNumber), null);

    public static void LogValidationFailed(this ILogger logger, string requestType, string errors)
        => _logValidationFailed(logger, requestType, errors, null);

    private static string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 4)
            return "***";

        return phoneNumber[..4] + new string('*', phoneNumber.Length - 4);
    }
}
