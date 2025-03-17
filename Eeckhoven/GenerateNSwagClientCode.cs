
using System.Net;
using NSwag;
using NSwag.CodeGeneration.TypeScript;

namespace Eeckhoven;
public class GenerateNSwagClientCode
{
    public async void Run()
    {
        var jsonFile = await File.ReadAllTextAsync("./swaggerfile.json");
        var document = await OpenApiDocument.FromJsonAsync(jsonFile);
        var settings = new TypeScriptClientGeneratorSettings
        {
            ClassName = "{controller}Client",
        };

        
        var generator = new TypeScriptClientGenerator(document, settings);
        var code = generator.GenerateFile();
        if (code != null)
        { 
            // Add custom header logic for Authorization to the generated TypeScript code
            var authorizationHeader = "'Authorization': 'Bearer ' + localStorage.getItem('token'),";
            var headerSearch = "headers: {";
            code = code.Replace(headerSearch, headerSearch + authorizationHeader);
            await File.WriteAllTextAsync(@"../../../WebstormProjects/eeckhoven/src/ClientApi.ts", code);
        }
    }
}