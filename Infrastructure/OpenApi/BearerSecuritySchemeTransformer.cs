using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TaskFlow.Infrastructure.OpenApi;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Name = "Authorization",
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", securityScheme);

        foreach (var path in document.Paths.Values)
        {
            if (path.Operations == null) continue;

            foreach (var operation in path.Operations.Values)
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });
            }
        }

        return Task.CompletedTask;
    }
}
