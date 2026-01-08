using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ilon.Modules.Identity.Features.SendOtp;

/// <summary>
/// Minimal API endpoint for sending OTP.
/// </summary>
public static class SendOtpEndpoint
{
    public static void MapSendOtpEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/send-otp", HandleAsync)
            .WithName("SendOtp")
            .WithTags("Authentication")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Send OTP to phone number";
                operation.Description = "Generates and sends a One-Time Password (OTP) to the specified phone number for authentication.";
                return operation;
            })
            .Produces<SendOtpResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        SendOtpRequest request,
        SendOtpHandler handler,
        SendOtpValidator validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors });
        }

        // Handle request
        var result = await handler.HandleAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { errors = result.Errors });
        }

        return Results.Ok(result.Data);
    }
}
