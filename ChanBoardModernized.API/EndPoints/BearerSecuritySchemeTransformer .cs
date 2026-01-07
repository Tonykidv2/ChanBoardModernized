using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ChanBoardModernized.API.EndPoints;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    //public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    //{
    //    var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
    //    if (!authenticationSchemes.Any(a => a.Name == "Bearer"))
    //        return;

    //    var bearerScheme = new OpenApiSecurityScheme
    //    {
    //        Type = SecuritySchemeType.Http,
    //        Scheme = "bearer",
    //        BearerFormat = "JWT",
    //        In = ParameterLocation.Header,
    //        Description = "JWT Authorization header using the Bearer scheme."
    //    };

    //    document.Components ??= new OpenApiComponents();

    //    document.AddComponent("Bearer", bearerScheme);

    //    var securityRequirement = new OpenApiSecurityRequirement
    //    {
    //        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    //    };

    //    foreach (var path in document.Paths.Values)
    //    {
    //        foreach (var operation in path.Operations.Values)
    //        {
    //            operation.Security ??= new List<OpenApiSecurityRequirement>();
    //            operation.Security.Add(securityRequirement);
    //        }
    //    }
    //}

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }] = Array.Empty<string>()
                });
            }
        }
    }
}
