using Ilon.Modules.Identity;
using Ilon.Modules.Identity.Features.SendOtp;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/ilon-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add memory cache for OTP storage (in production, use Redis)
builder.Services.AddMemoryCache();

// Add health checks
builder.Services.AddHealthChecks();

// TODO: Add rate limiting in future iteration
// builder.Services.AddRateLimiter(...);

// Register modules
builder.Services.AddIdentityModule();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ilon API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
// app.UseRateLimiter(); // TODO: Add rate limiting

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

// Map module endpoints
app.MapSendOtpEndpoint();

// Root endpoint
app.MapGet("/", () => new
{
    Application = "Ilon API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Endpoints = new[]
    {
        "/swagger - API Documentation",
        "/health - Health check",
        "/api/auth/send-otp - Send OTP"
    }
})
.WithName("Root")
.WithTags("Info")
.ExcludeFromDescription();

app.Run();
