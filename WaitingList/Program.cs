using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WaitingList.Database;
using WaitingList.Repositories;
using WaitingList.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSwag;
using NSwag.Generation.Processors.Security;
using WaitingList.BackgroundServices;
using WaitingList.Extensions;
using WaitingList.Interfaces;
using WaitingList.Services;
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
builder.Services.AddHostedService<EnsureBackgroundExistsBackgroundService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IWaitingListRepository, WaitingListRepository>();
builder.Services.AddScoped<IPartyRepository, PartyRepository>();
builder.Services.AddScoped<IWaitingListService, WaitingListService>();
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString ?? "", mySqlOptions => mySqlOptions.MigrationsHistoryTable("__EFMigrationsHistory")));

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
    
   app.Services.GenerateApiJson();
}
app.UseHttpsRedirection();
app.MapControllers();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.Run();