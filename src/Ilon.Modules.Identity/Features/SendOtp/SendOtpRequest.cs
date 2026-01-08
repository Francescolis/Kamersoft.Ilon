namespace Ilon.Modules.Identity.Features.SendOtp;

/// <summary>
/// Request to send an OTP to a phone number.
/// </summary>
public sealed record SendOtpRequest(string PhoneNumber);

/// <summary>
/// Response containing OTP send confirmation.
/// </summary>
public sealed record SendOtpResponse(string Message, DateTimeOffset ExpiresAt);
