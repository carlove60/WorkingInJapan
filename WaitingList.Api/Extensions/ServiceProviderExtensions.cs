
namespace WaitingList.Extensions;

using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;

/// <summary>
/// 
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Generates a JSON file with all API calls and necessary models
    /// </summary>
    /// <param name="provider"></param>
    public static void GenerateSwaggerApiJson(this IServiceProvider provider)
    {
        var sw = provider.GetRequiredService<ISwaggerProvider>();
        var doc = sw.GetSwagger("v1", null, "/");
        var swaggerFile = doc.SerializeAsJson(Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);
        File.WriteAllText("../../../WebstormProjects/eeckhoven/swaggerfile.json", swaggerFile);
       // _ = GenerateNSwagClientCode.Run();
    }
}