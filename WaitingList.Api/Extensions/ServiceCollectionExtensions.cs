using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WaitingList.BackgroundServices.BackgroundServices;
using WaitingList.Database.Database;
using WaitingList.Swagger;
using WaitingListBackend.Interfaces;
using WaitingListBackend.Repositories;
using WaitingListBackend.Services;
using OpenApiSecurityScheme = Microsoft.OpenApi.Models.OpenApiSecurityScheme;
using OpenApiServer = Microsoft.OpenApi.Models.OpenApiServer;

namespace WaitingList.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the Dependency Injection (DI) container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures user session settings, including idle timeout, cookie options,
    /// and distributed memory cache for session state management within the application.
    /// </summary>
    /// <param name="services">The service collection to which the session configuration will be added.</param>
    /// <returns>The service collection with session configuration applied.</returns>
    public static IServiceCollection AddUserSession(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.Name = WaitingListBackend.Constants.WaitingListSessionKey;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax; // For Chrome
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        });
        return services;
    }

    /// <summary>
    /// Adds required services and repositories to the service collection,
    /// including hosted services, API explorers,
    /// scoped services, and repository implementations for DI.
    /// </summary>
    /// <param name="services">The service collection to which the services and repositories will be added.</param>
    /// <returns>The service collection with the required services and repositories configured.</returns>
    public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
    {
        services.AddHostedService<EnsureWaitingListExistsBackgroundService>();
        services.AddHostedService<ConcludeServiceBackgroundService>();
        services.AddHostedService<DeleteTimedOutSessionsBackgroundService>();
        services.AddEndpointsApiExplorer();
        services.AddScoped<IWaitingListService, WaitingListService>();
        services.AddScoped<IWaitingListRepository, WaitingListRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IPartyService, PartyService>();
        return services;
    }

    /// <summary>
    /// Configures the database connection for the application using the provided connection string.
    /// </summary>
    /// <param name="services">The service collection to which the database connection will be added.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <returns>The service collection with the database connection configuration added.</returns>
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySQL(connectionString, mySqlOptions => mySqlOptions.MigrationsHistoryTable("__EFMigrationsHistory")));

        return services;
    }

    /// <summary>
    /// Configures Swagger generation and adds related settings to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which Swagger generation will be added.</param>
    /// <returns>The service collection with the Swagger configuration added.</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SchemaFilter<EnumSchemaFilter>();
            c.AddServer(new OpenApiServer { Url = "http://localhost:5240" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme.",
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
            c.DescribeAllParametersInCamelCase();
            c.SupportNonNullableReferenceTypes();
        });
        return services;
    }

    /// <summary>
    /// Adds CORS policy to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the CORS configuration will be added.</param>
    /// <returns>The service collection with the CORS configuration added.</returns>
    public static IServiceCollection AddWebCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(WaitingList.Constants.ApiCallCorsPolicy,
                policy => 
                    policy.WithOrigins(
                            "http://localhost:5173",  // Vite default
                            "http://localhost:5174",  // Vite now
                            "http://localhost:5240",   // Local
                            "http://127.0.0.1:5173",
                            "http://127.0.0.1:5174",
                            "http://127.0.0.1:5240"
                        )
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
            );
        });
        return services;
    }
}