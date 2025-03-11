using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.BoxAPI.Core.Services;
using US.BOX.BoxAPI.Data;
using US.BOX.DebtorPortalAPI.Swagger;
using US.BOX.DebtorPortalAPI.Utils;
using Serilog;
using US.BOX.DebtorPortalAPI.Providers;
using US.BOX.BoxAPI.Data.Models;
using DinkToPdf.Contracts;
using DinkToPdf;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);
IConfiguration _configuration;
var configBuilder = new ConfigurationBuilder().AddEnvironmentVariables();
string env = Environment.GetEnvironmentVariable("Env");
string service = Environment.GetEnvironmentVariable("Service");
string endpoint = Environment.GetEnvironmentVariable("AppConfigEndpoint");
string clientId = Environment.GetEnvironmentVariable("ClientId");

configBuilder.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(endpoint), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId }))
            .Select("Common*", LabelFilter.Null)
            .Select("Common*", env)
            .Select(service + "*", LabelFilter.Null)
            .Select(service + "*", env)
            .UseFeatureFlags();
});
builder.Services.AddFeatureManagement();

_configuration = configBuilder.Build();
DPAPIConfig.ClientID = clientId;
DPAPIConfig.CatalogDBConnectionString = _configuration["Common:WorkstationDbConnection"];
DPAPIConfig.AuthSecret = _configuration["DPAPI:AuthSettings:Secret"];
DPAPIConfig.AllowedHosts = _configuration["DPAPI:AuthSettings:AllowedHosts"];
DPAPIConfig.AuthValidAudience = _configuration["DPAPI:AuthSettings:ValidAudience"];
DPAPIConfig.AuthValidIssuer = _configuration["DPAPI:AuthSettings:ValidIssuer"];
DPAPIConfig.AzureBlobStorageContainerName = _configuration["DPAPI:AzureBlobStorageContainerName"];
DPAPIConfig.AzureBlobStorageAccountName = _configuration["DPAPI:AzureBlobStoragName"];
//DPAPIConfig.ActivityExecutionAPIURL = _configuration["DPAPI:ActivityExecutionAPIURL"];
//DPAPIConfig.ActivityExecutionAPIUser = _configuration["DPAPI:ActivityExecutionAPIUser"];
//DPAPIConfig.ActivityExecutionAPIUSerCode = _configuration["DPAPI:ActivityExecutionAPIUSerCode"];
DPAPIConfig.GenerateGDPRActivityCode = _configuration["DPAPI:GenerateGDPRActivityCode"];
DPAPIConfig.InvoiceActivityCode = _configuration["DPAPI:InvoiceActivityCode"];
DPAPIConfig.CPReciptTemplate = _configuration["DPAPI:CPReciptTemplate"];
DPAPIConfig.EncryptionKey = _configuration["DPAPI:ActivityExecutionAPIEncriptionKey"];
DPAPIConfig.ReciptFolderpath = _configuration["DPAPI:ReciptFolderpath"];
DPAPIConfig.TenentID = _configuration["DPAPI:TenentID"];


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // This will read the sinks from appsettings.json
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add CORS policy before services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keeps the original property names
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DebtorPortalAPI - V1", Version = "1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "DebtorPortalAPI - V2", Version = "2" });

    c.DocumentFilter<AuthTokenOperation>();

    // Add this line
    // c.OperationFilter<ConfigureSwaggerOptions>();

    // To Enable authorization using Swagger (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}

                    }
                });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(DPAPIConfig.AuthSecret);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true, // Enable Issuer validation
            ValidIssuer = DPAPIConfig.AuthValidIssuer, // Specify the expected Issuer value (e.g., "https://your-auth-server.com")
            ValidateAudience = true, // Enable Audience validation
            ValidAudience = DPAPIConfig.AuthValidAudience, // Specify the expected Audience value (e.g., "https://your-api.com")
            ValidateLifetime = true, // Enable token expiry validation
        };
    });

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDebtorPortalAPIService, DebtorPortalAPIService>();
builder.Services.AddSingleton<IDataManager, DataManager>();
builder.Services.AddSingleton<ITools, PdfTools>();
builder.Services.AddSingleton<IConverter, BasicConverter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "DebtorPortalAPI - V1");
        options.SwaggerEndpoint("v2/swagger.json", "DebtorPortalAPI - V2");
        options.RoutePrefix = "swagger";
    });
}
app.UseCors("AllowAllOrigins");
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();  // Log HTTP requests
app.UseCancellationHandling();
app.UseHttpsRedirection();
app.UseAuthentication(); // Enable JWT authentication
app.UseAuthorization();
app.MapControllers();

app.Run();
