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


var builder = WebApplication.CreateBuilder(args);

// Set up Serilog configuration to read from appsettings.json
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
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();
        var key = Encoding.ASCII.GetBytes(authSettings.Secret);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true, // Enable Issuer validation
            ValidIssuer = authSettings.ValidIssuer, // Specify the expected Issuer value (e.g., "https://your-auth-server.com")
            ValidateAudience = true, // Enable Audience validation
            ValidAudience = authSettings.ValidAudience, // Specify the expected Audience value (e.g., "https://your-api.com")
            //  ValidateIssuer = false,
            // ValidateAudience = false
            ValidateLifetime = true, // Enable token expiry validation

        };
    });


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDebtorPortalAPIService, DebtorPortalAPIService>();
builder.Services.AddSingleton<IDataManager , DataManager>();

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
