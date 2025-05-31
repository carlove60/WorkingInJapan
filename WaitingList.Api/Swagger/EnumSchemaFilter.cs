using Microsoft.OpenApi.Any;

namespace WaitingList.Swagger;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

/// <summary>
/// A schema filter that modifies the OpenAPI schema for enumeration
/// Allowing for strings to be set
/// </summary>
/// <remarks>
/// The <c>EnumSchemaFilter</c> class implements the <c>ISchemaFilter</c> interface to
/// customize the OpenAPI schema generation for enumeration types. This filter ensures
/// that enumerations are presented as string values in the OpenAPI documentation.
/// </remarks>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Type = nameof(String);
            schema.Enum.Clear();
            foreach (var name in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}