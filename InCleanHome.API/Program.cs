using InCleanHome.API.Booking.Application.Internal.CommandServices;
using InCleanHome.API.Booking.Application.Internal.QueryServices;
using InCleanHome.API.Booking.Domain.Repositories;
using InCleanHome.API.Booking.Domain.Services;
using InCleanHome.API.Booking.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.IAM.Application.Internal.CommandServices;
using InCleanHome.API.IAM.Application.Internal.OutboundServices;
using InCleanHome.API.IAM.Application.Internal.QueryServices;
using InCleanHome.API.IAM.Domain.Repositories;
using InCleanHome.API.IAM.Domain.Services;
using InCleanHome.API.IAM.Infrastructure.Hashing.BCrypt.Services;
using InCleanHome.API.IAM.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using InCleanHome.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using InCleanHome.API.IAM.Infrastructure.Tokens.JWT.Services;
using InCleanHome.API.IAM.Interfaces.ACL;
using InCleanHome.API.IAM.Interfaces.ACL.Services;
using InCleanHome.API.Messaging.Application.Internal.CommandServices;
using InCleanHome.API.Messaging.Application.Internal.QueryServices;
using InCleanHome.API.Messaging.Domain.Repositories;
using InCleanHome.API.Messaging.Domain.Services;
using InCleanHome.API.Messaging.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.Payments.Application.Internal.CommandServices;
using InCleanHome.API.Payments.Application.Internal.QueryServices;
using InCleanHome.API.Payments.Domain.Repositories;
using InCleanHome.API.Payments.Domain.Services;
using InCleanHome.API.Payments.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.Profiles.Application.ACL;
using InCleanHome.API.Profiles.Application.Internal.CommandServices;
using InCleanHome.API.Profiles.Application.Internal.QueryServices;
using InCleanHome.API.Profiles.Domain.Repositories;
using InCleanHome.API.Profiles.Domain.Services;
using InCleanHome.API.Profiles.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.Profiles.Interfaces.ACL;
using InCleanHome.API.ReviewsAndEvaluation.Application.Internal.CommandServices;
using InCleanHome.API.ReviewsAndEvaluation.Application.Internal.QueryServices;
using InCleanHome.API.ReviewsAndEvaluation.Domain.Repositories;
using InCleanHome.API.ReviewsAndEvaluation.Domain.Services;
using InCleanHome.API.ReviewsAndEvaluation.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.SearchAndCatalog.Application.Internal.CommandServices;
using InCleanHome.API.SearchAndCatalog.Application.Internal.QueryServices;
using InCleanHome.API.SearchAndCatalog.Domain.Repositories;
using InCleanHome.API.SearchAndCatalog.Domain.Services;
using InCleanHome.API.SearchAndCatalog.Infrastructure.Persistence.EFC.Repositories;
using InCleanHome.API.Shared.Domain.Repositories;
using InCleanHome.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// Routing & Controllers
// ============================================================================
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// ============================================================================
// CORS — fully open for the Vite dev server (refine in production)
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ============================================================================
// PostgreSQL via Npgsql
// ============================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseNpgsql(connectionString)
               .LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors();
    else
        options.UseNpgsql(connectionString)
               .LogTo(Console.WriteLine, LogLevel.Error);
});

// ============================================================================
// Swagger / OpenAPI
// ============================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "InCleanHome.API",
        Version     = "v1",
        Description = "InCleanHome — Domestic Service Hiring Platform API",
        Contact     = new OpenApiContact { Name = "InCleanHome", Email = "contact@incleanhome.pe" },
        License     = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url  = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In           = ParameterLocation.Header,
        Description  = "Please enter the JWT token (without the 'Bearer ' prefix)",
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme       = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
            },
            Array.Empty<string>()
        }
    });
    options.EnableAnnotations();
});

// ============================================================================
// Dependency Injection — per Bounded Context
// ============================================================================

// Shared
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorkerDocumentRepository, WorkerDocumentRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Profiles
builder.Services.AddScoped<IClientProfileRepository, ClientProfileRepository>();
builder.Services.AddScoped<IWorkerProfileRepository, WorkerProfileRepository>();
builder.Services.AddScoped<IClientProfileCommandService, ClientProfileCommandService>();
builder.Services.AddScoped<IClientProfileQueryService, ClientProfileQueryService>();
builder.Services.AddScoped<IWorkerProfileCommandService, WorkerProfileCommandService>();
builder.Services.AddScoped<IWorkerProfileQueryService, WorkerProfileQueryService>();
builder.Services.AddScoped<IProfilesContextFacade, ProfilesContextFacade>();

// SearchAndCatalog
builder.Services.AddScoped<IAvailabilitySlotRepository, AvailabilitySlotRepository>();
builder.Services.AddScoped<IAvailabilitySlotCommandService, AvailabilitySlotCommandService>();
builder.Services.AddScoped<IAvailabilitySlotQueryService, AvailabilitySlotQueryService>();

// Booking
builder.Services.AddScoped<IBookingRequestRepository, BookingRequestRepository>();
builder.Services.AddScoped<IBookingRequestCommandService, BookingRequestCommandService>();
builder.Services.AddScoped<IBookingRequestQueryService, BookingRequestQueryService>();

// Payments
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodCommandService, PaymentMethodCommandService>();
builder.Services.AddScoped<IPaymentMethodQueryService, PaymentMethodQueryService>();

// ReviewsAndEvaluation
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewCommandService, ReviewCommandService>();
builder.Services.AddScoped<IReviewQueryService, ReviewQueryService>();

// Messaging
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageCommandService, MessageCommandService>();
builder.Services.AddScoped<IMessageQueryService, MessageQueryService>();

var app = builder.Build();

// ============================================================================
// Database initialization (auto-create on first run)
// ============================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context  = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// ============================================================================
// HTTP request pipeline
// ============================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");
app.UseRequestAuthorization();
app.MapControllers();

app.Run();
