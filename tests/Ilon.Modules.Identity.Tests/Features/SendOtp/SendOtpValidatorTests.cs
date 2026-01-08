using Ilon.Modules.Identity.Features.SendOtp;

namespace Ilon.Modules.Identity.Tests.Features.SendOtp;

public class SendOtpValidatorTests
{
    private readonly SendOtpValidator _validator = new();

    [Fact]
    public void Validate_ValidPhoneNumber_ReturnsValid()
    {
        // Arrange
        var request = new SendOtpRequest("+237612345678");

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_EmptyPhoneNumber_ReturnsInvalid()
    {
        // Arrange
        var request = new SendOtpRequest("");

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("required", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_InvalidPhoneNumber_ReturnsInvalid()
    {
        // Arrange
        var request = new SendOtpRequest("1234567890");

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Invalid phone number format", result.Errors[0]);
    }

    [Fact]
    public void Validate_PhoneNumberWithoutPlus_ReturnsInvalid()
    {
        // Arrange
        var request = new SendOtpRequest("237612345678");

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}
