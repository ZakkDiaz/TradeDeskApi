using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using TradeDeskApi.Common.Authorization;
using TradeDeskBroker;
using TradeDeskData;
using TradeDeskTop.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("secrets.json");
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(DatabaseConfig.ConfigName));

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKeyAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Description = "Enter your API key"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "ApiKeyAuth",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Add authentication services
builder.Services.AddAuthentication("ApiKeyAuth")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyAuth", null);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IApiKeyService, ApiKeyService>();
builder.Services.AddSingleton<IBrokerageService,  BrokerageService>();
builder.Services.AddSingleton<IFinancialRepository, FinancialRepository>();
var app = builder.Build();

app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Make sure this comes before UseAuthorization()
app.UseAuthorization();

app.MapControllers().RequireAuthorization();
app.MapFallbackToFile("index.html");
app.Run();