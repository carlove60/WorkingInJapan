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
            });
            var configuration = builder.Configuration;
            var connectionString = configuration.GetConnectionString("MySqlConnection") ?? "";
            builder.Services.AddOpenApi()
                .AddSwagger()
                .AddDatabaseConnection(connectionString)
                .AddServicesAndRepositories()
                .AddUserSession()
                .AddWebCors();
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

                app.Services.GenerateSwaggerApiJson();
            }

            app.UseHttpsRedirection();
            app.UseHttpsRedirection();
            app.UseCors(Constants.ApiCallCorsPolicy);
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.UseSessionMiddleware();
            app.useSessionTrackingMiddleware();
            app.MapControllers();
            app.Run();
        }
    }
}