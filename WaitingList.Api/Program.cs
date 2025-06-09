using System.Text.Json;
using System.Text.Json.Serialization;
using WaitingList.Extensions;
using WaitingList.Middleware;

namespace WaitingList;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            var configuration = builder.Configuration;
            var connectionString = configuration.GetConnectionString("MySqlConnection") ?? "";
            builder.Services.AddOpenApi()
                .AddSwagger()
                .AddDatabaseConnection(connectionString)
                .AddServicesAndRepositories()
                .AddUserSession()
                .AddWebCors();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10); // Extend keep-alive for SSE
            });

        }
        
        var app = builder.Build();
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });

                //app.Services.GenerateSwaggerApiJson();
            }
            app.UseRouting();
            app.UseSession();
            app.UseCors(Constants.ApiCallCorsPolicy);
            app.UseAuthorization();
            app.UseSessionMiddleware();
            app.MapControllers();
            app.Run();
        }
    }
}