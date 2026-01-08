using Ilon.BuildingBlocks.Observability;
using Ilon.BuildingBlocks.Primitives;
using Ilon.BuildingBlocks.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ilon.Modules.Identity.Features.SendOtp;

public interface IOperationResult<T>
{
    bool IsSuccess { get; }
    T? Data { get; }
    IEnumerable<string> Errors { get; }
}

public sealed class OperationResult<T> : IOperationResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public int StatusCode { get; init; }
}

/// <summary>
/// Handler for sending OTP to a phone number.
/// </summary>
public sealed class SendOtpHandler
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<SendOtpHandler> _logger;
    private const int OtpExpirationMinutes = 5;
    private const int OtpLength = 6;

    public SendOtpHandler(IMemoryCache cache, ILogger<SendOtpHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<IOperationResult<SendOtpResponse>> HandleAsync(
        SendOtpRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validate phone number format
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumber is null)
        {
            return Task.FromResult<IOperationResult<SendOtpResponse>>(new OperationResult<SendOtpResponse>
            {
                IsSuccess = false,
                Errors = new[] { "Invalid phone number format. Use international format (e.g., +237XXXXXXXXX)" },
                StatusCode = 400
            });
        }

        // Generate OTP
        var otp = OtpGenerator.Generate(OtpLength);

        // Calculate expiration
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(OtpExpirationMinutes);

        // Store OTP in cache with expiration
        var cacheKey = $"otp:{phoneNumber.Value}";
        _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(OtpExpirationMinutes));

        // Log the operation (in production, this would send SMS)
        _logger.LogOtpSent(phoneNumber.Value);
        _logger.LogInformation("OTP for {PhoneNumber}: {Otp} (Development only - remove in production)", 
            phoneNumber.Value, otp);

        // In production, you would integrate with an SMS provider here
        // await _smsService.SendAsync(phoneNumber, otp, cancellationToken);

        var response = new SendOtpResponse(
            Message: $"OTP sent to {phoneNumber.Value}. Valid for {OtpExpirationMinutes} minutes.",
            ExpiresAt: expiresAt
        );

        return Task.FromResult<IOperationResult<SendOtpResponse>>(new OperationResult<SendOtpResponse>
        {
            IsSuccess = true,
            Data = response,
            StatusCode = 200
        });
    }
}
