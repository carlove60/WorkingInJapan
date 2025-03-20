using System.Text;
using System.Text.Json.Serialization;
using Eeckhoven.ApplicationSignInManager;
using Eeckhoven.ApplicationUserManager;
using Eeckhoven.Authorization;
using Microsoft.EntityFrameworkCore;
using Eeckhoven.Database;
using Eeckhoven.Extensions;
using Eeckhoven.Middleware;
using Eeckhoven.Repositories;
using Eeckhoven.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenApiSecurityScheme = Microsoft.OpenApi.Models.OpenApiSecurityScheme;
using OpenApiServer = Microsoft.OpenApi.Models.OpenApiServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
    config.Title = "My API";
    
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<ClassRepository, ClassRepository>();
builder.Services.AddScoped<LessonRepository, LessonRepository>();
builder.Services.AddScoped<LanguageRepository, LanguageRepository>();
builder.Services.AddScoped<ApplicationPasswordValidator, ApplicationPasswordValidator>();
builder.Services.AddScoped<ApplicationUserManager>();
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString, mySqlOptions => mySqlOptions.MigrationsHistoryTable("__EFMigrationsHistory")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<ApplicationUserManager>()
    .AddSignInManager<ApplicationSignInManager>()
    .AddPasswordValidator<ApplicationPasswordValidator>();

builder.Services.AddTransient<ICustomUserValidator<ApplicationUser>, CustomUserValidator<ApplicationUser>>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Session-Status", "X-Custom-Header") // Allow headers to be read
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

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/user/login"; // Specify the login page
    });

var app = builder.Build();
app.UseCors("AllowAll");

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
    
    app.Services.CreateClientApi();
}
app.UseMiddleware<JwtMiddleware>(); // Auto-refresh token on activity

// Enable authentication
app.UseAuthentication();

// Enable authorization
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
var scope = app.Services.CreateScope();
try
{
    var services = scope.ServiceProvider;
    await RoleTask.SeedRoles(services);
}
finally
{
    scope.Dispose();
}