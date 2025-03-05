using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace US.BOX.DebtorPortalAPI.Swagger;

public class AuthTokenOperation : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var version = swaggerDoc.Info.Version;
        var path = $"/v{version}/auth/token";

        var pathItem = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = new OpenApiOperation
                {
                    Summary = "Generate Authentication Key",
                    Description = "<h3>Remarks</h3> Use username and password that unicorn box has given and use <b>password</b> as grant_type. AppId, Key1 and Company are not mandatory",
                    Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Auth" } },
                    RequestBody = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["username"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Registerd username of the application. In the Debtor portal it is Case Number.",
                                            Required = new HashSet<string> { "username" }
                                        },
                                        ["password"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Password of the user",
                                            Required = new HashSet<string> { "password" }
                                        },
                                        ["company"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Provided company Id"
                                        }
                                      
                                    }
                                }
                            }
                        }
                    },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "Please use JWT access token to authorize BOXAPI",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["access_token"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            },
                                            ["token_type"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            },
                                            ["expires_in"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        if (swaggerDoc.Paths.ContainsKey(path))
        {
            swaggerDoc.Paths.Remove(path);
            swaggerDoc.Paths.Add(path, pathItem);
        }
    }
}