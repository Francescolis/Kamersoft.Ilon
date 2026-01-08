namespace Ilon.Modules.Identity.Features.VerifyOtp;

/// <summary>
/// Request to verify an OTP for a phone number.
/// </summary>
public sealed record VerifyOtpRequest(string PhoneNumber, string Otp);

/// <summary>
/// Response containing OTP verification result.
/// </summary>
public sealed record VerifyOtpResponse(bool IsValid, string? Token = null);
