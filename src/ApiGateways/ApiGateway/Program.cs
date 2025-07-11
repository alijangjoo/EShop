using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Explicitly load Ocelot configuration files (base + environment specific)
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();

// Replace the existing AddOcelot() registration so it uses the updated configuration
builder.Services.AddOcelot(builder.Configuration)
                .AddPolly()
                .AddConsul();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EShop API Gateway",
        Version = "v1",
        Description = "API Gateway for EShop Microservices Platform / دروازه API برای پلتفرم میکروسرویس ای‌شاپ",
        Contact = new OpenApiContact
        {
            Name = "EShop Team",
            Email = "support@eshop.com"
        }
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EShop API Gateway v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "EShop API Gateway";
    });
}

// Enable CORS
app.UseCors("AllowAll");

// Add path normalization middleware to handle double slashes
app.Use(async (context, next) =>
{
    var originalPath = context.Request.Path.Value;
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, originalPath);
    
    if (originalPath != null && originalPath.Contains("//"))
    {
        var normalizedPath = originalPath.Replace("//", "/");
        context.Request.Path = normalizedPath;
        logger.LogInformation("Normalized path from {OriginalPath} to {NormalizedPath}", originalPath, normalizedPath);
    }
    
    // Log the final path that will be processed by Ocelot
    logger.LogInformation("Final path for Ocelot: {Method} {Path}", context.Request.Method, context.Request.Path);
    
    await next();
    
    logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode}", 
        context.Request.Method, context.Request.Path, context.Response.StatusCode);
});

// Add Authentication middleware; we deliberately omit global Authorization middleware so that
// Ocelot handles route-level authorization based on its configuration. This prevents
// unauthenticated errors for public endpoints like /api/auth/register.
app.UseAuthentication();

// Add Health Check endpoint
app.MapHealthChecks("/health");

// Add a simple endpoint to test the gateway
app.MapGet("/", () => new
{
    Service = "EShop API Gateway",
    Version = "1.0.0",
    Status = "Running",
    Environment = app.Environment.EnvironmentName,
    DateTime = DateTime.UtcNow,
    Message = "Welcome to EShop API Gateway / خوش آمدید به دروازه API ای‌شاپ",
    Documentation = "/swagger",
    Health = "/health"
});

// Use Ocelot middleware
await app.UseOcelot();

app.Run();