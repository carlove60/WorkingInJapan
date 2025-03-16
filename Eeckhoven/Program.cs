using System.Text;
using Eeckhoven.Authorization;
using Microsoft.EntityFrameworkCore;
using Eeckhoven.Database;
using Eeckhoven.Extensions;
using Eeckhoven.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<ClassRepository, ClassRepository>();
builder.Services.AddScoped<LessonRepository, LessonRepository>();
builder.Services.AddScoped<LanguageRepository, LanguageRepository>();

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString, mySqlOptions => mySqlOptions.MigrationsHistoryTable("__EFMigrationsHistory")
    ));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // For development (use true in production)
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