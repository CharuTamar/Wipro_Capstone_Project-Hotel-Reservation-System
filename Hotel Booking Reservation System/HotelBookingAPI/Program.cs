using HotelBookingAPI.Docs; // Use docs namespace
using HotelBookingAPI.Models;
using HotelBookingAPI.Data;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using HotelBookingAPI.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
});


var logger = loggerFactory.CreateLogger<Program>();

var builder = WebApplication.CreateBuilder(args);


//  Map "sub" and "nameidentifier" to ClaimTypes.NameIdentifier
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["sub"] = ClaimTypes.NameIdentifier;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] = ClaimTypes.NameIdentifier;


// Enable CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy => policy
            .WithOrigins(
                "http://localhost:5173", //  Frontend URL
                "https://checkout.stripe.com" //  Stripe URL
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); //  Required if you're using JWT or cookies
});


//  Configure logging before adding services
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Enable console logging
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Set log level

//  Add services for controllers and Swagger
builder.Services.AddOpenApi();
builder.Services.AddControllers();

//  Register Services and Interfaces
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<PaymentService>();


//  Configure DbContext and Identity
builder.Services.AddDbContext<HotelBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<HotelBookingDbContext>()
    .AddDefaultTokenProviders();

//  Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            //  Read token from cookie if Authorization header is missing
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token))
                {
                    context.Token = context.Request.Cookies["token"];
                }
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;

                if (claimsIdentity != null)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        logger.LogInformation($"➡️ Claim Type: {claim.Type}, Value: {claim.Value}");
                    }
                }

                if (!claimsIdentity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    var subClaim = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                    if (!string.IsNullOrEmpty(subClaim))
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, subClaim));
                        logger.LogInformation($" Manually mapped User ID to ClaimTypes.NameIdentifier: {subClaim}");
                    }
                }


                var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    logger.LogInformation($" User ID resolved from token: {userId}");
                }
                else
                {
                    logger.LogWarning("❌ User ID claim is missing or not mapped correctly.");
                }


                //  Log roles for debugging
                var roleClaims = claimsIdentity?.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (roleClaims != null && roleClaims.Any())
                {
                    foreach (var role in roleClaims)
                    {
                        logger.LogInformation($" User role: {role}");
                    }
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>()
                    .LogError($"❌ Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }

        };
    });


//  Add Swagger using extension method from `docs` folder
builder.Services.ConfigureSwagger();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins"); // Apply the CORS policy globally

//  Seed Data (Role creation) with error handling
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.SeedRolesAsync(services);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error while seeding roles"); //  Reusing existing logger
    }
}


logger.LogInformation("➡️ Application starting...");

//  Request logging
app.Use(async (context, next) =>
{
    logger.LogInformation($"➡️ Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
    logger.LogInformation($"⬅️ Response status: {context.Response.StatusCode}");
});

//  Enable Authentication and Authorization
app.UseAuthentication();

//  Register middleware in the correct order
// 🔥 Keep `CustomAuthMiddleware` OR `RoleAuthorizationMiddleware` — NOT BOTH
// app.UseMiddleware<CustomAuthMiddleware>();
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.UseAuthorization();

//  Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerSetup();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//  Map Controllers
app.MapControllers();

logger.LogInformation(" Application started and running!");

app.Run();
