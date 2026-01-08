using Ilon.BuildingBlocks.Primitives;

namespace Ilon.Modules.Identity.Features.SendOtp;

/// <summary>
/// Validator for SendOtpRequest.
/// </summary>
public sealed class SendOtpValidator
{
    public (bool IsValid, string[] Errors) Validate(SendOtpRequest instance)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(instance.PhoneNumber))
        {
            errors.Add("Phone number is required");
        }
        else if (!PhoneNumber.IsValid(instance.PhoneNumber))
        {
            errors.Add("Invalid phone number format. Use international format (e.g., +237XXXXXXXXX for Cameroon)");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}
