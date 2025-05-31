

using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSwag;
using NSwag.Generation.Processors.Security;
using WaitingList.Extensions;
using WaitingList.Middleware;
using WaitingList.Swagger;
using WaitingListBackend;
using WaitingListBackend.BackgroundServices;
using WaitingListBackend.Database;
using WaitingListBackend.Interfaces;
using WaitingListBackend.Repositories;
using WaitingListBackend.Services;
using OpenApiSecurityScheme = Microsoft.OpenApi.Models.OpenApiSecurityScheme;
using OpenApiServer = Microsoft.OpenApi.Models.OpenApiServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSwaggerGen(c =>
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
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Waiting list API";
    
    // Add JWT Bearer Security
    config.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type 'Bearer {your JWT token}'"
    });

    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString ?? "", mySqlOptions => mySqlOptions.MigrationsHistoryTable("__EFMigrationsHistory")));

builder.Services.AddHostedService<EnsureBackgroundExistsBackgroundService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IWaitingListService, WaitingListService>();
builder.Services.AddScoped<IWaitingListRepository, WaitingListRepository>();
builder.Services.AddScoped<IPartyRepository, PartyRepository>();
builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.Name = Constants.WaitingListSessionKey;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // ✅ Important for Chrome
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Or Always for HTTPS
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => 
            policy.WithOrigins(
                    "http://localhost:3000",  // React
                    "http://localhost:4200",  // Angular
                    "http://localhost:5173",  // Vite default
                    "http://localhost:5174"   // Vite now
                )
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
        );
});
var configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // For development (use true in production)
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    
   app.Services.GenerateSwaggerApiJson();
}
app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseSessionValidation();
app.MapControllers();
app.Run();