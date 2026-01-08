using Ilon.Modules.Identity.Features.SendOtp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ilon.Modules.Identity.Tests.Features.SendOtp;

public class SendOtpHandlerTests
{
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<SendOtpHandler>> _loggerMock;
    private readonly SendOtpHandler _handler;

    public SendOtpHandlerTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<SendOtpHandler>>();
        _handler = new SendOtpHandler(_cache, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidPhoneNumber_ReturnsSuccess()
    {
        // Arrange
        var request = new SendOtpRequest("+237612345678");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Contains("+237612345678", result.Data.Message);
        Assert.True(result.Data.ExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhoneNumber_ReturnsFailure()
    {
        // Arrange
        var request = new SendOtpRequest("invalid");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Invalid phone number format", result.Errors.First());
    }

    [Fact]
    public async Task HandleAsync_EmptyPhoneNumber_ReturnsFailure()
    {
        // Arrange
        var request = new SendOtpRequest("");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task HandleAsync_ValidPhoneNumber_StoresOtpInCache()
    {
        // Arrange
        var phoneNumber = "+237612345678";
        var request = new SendOtpRequest(phoneNumber);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        var cacheKey = $"otp:{phoneNumber}";
        var cachedOtp = _cache.Get<string>(cacheKey);
        Assert.NotNull(cachedOtp);
        Assert.Equal(6, cachedOtp.Length);
        Assert.All(cachedOtp, c => Assert.True(char.IsDigit(c)));
    }

    [Fact]
    public async Task HandleAsync_InternationalPhoneNumber_ReturnsSuccess()
    {
        // Arrange
        var request = new SendOtpRequest("+33612345678");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
