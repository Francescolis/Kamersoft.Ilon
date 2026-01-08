using Ilon.Modules.Identity.Features.SendOtp;
using Microsoft.Extensions.DependencyInjection;

namespace Ilon.Modules.Identity;

/// <summary>
/// Extension methods for registering the Identity module.
/// </summary>
public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        // Register handlers and validators
        services.AddScoped<SendOtpHandler>();
        services.AddScoped<SendOtpValidator>();

        return services;
    }
}
