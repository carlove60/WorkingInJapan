
using NSwag;
using NSwag.CodeGeneration.TypeScript;

namespace WaitingList;
public static class GenerateNSwagClientCode
{
    public static async Task Run()
    {
        var jsonFile = await File.ReadAllTextAsync("./swaggerfile.json");
        var document = await OpenApiDocument.FromJsonAsync(jsonFile);
        var settings = new TypeScriptClientGeneratorSettings
        {
            ClassName = "{controller}Client",
            Template = TypeScriptTemplate.Fetch,
            TypeScriptGeneratorSettings = 
            {
                TypeScriptVersion = 5.7M
            }        
        };

        
        var generator = new TypeScriptClientGenerator(document, settings);
        var code = generator.GenerateFile();
        if (code != null && code.Contains("JustDoIt"))
        { 
            // Add custom header logic for Authorization to the generated TypeScript code
            var authorizationHeader = "'Authorization': 'Bearer ' + localStorage.getItem('token'),";
            var headerSearch = "headers: {\n";
            code = code.Replace(headerSearch, $"{headerSearch}{authorizationHeader}");
            await File.WriteAllTextAsync(@"../../../WebstormProjects/eeckhoven/src/ClientApi.ts", code);
        }
    }
}