using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TradeDeskApi.Common.Authorization;
using TradeDeskBroker;
using TradeDeskBroker.Market;
using TradeDeskData;
using TradeDeskTop.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("secrets.json");
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(DatabaseConfig.ConfigName));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddResponseCaching();

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
builder.Services.AddSingleton<IMarketService, MarketService>();
builder.Services.AddSingleton<IIndicatorFactory, IndicatorFactory>();
builder.Services.AddSingleton<ITradeContext, TradeContext>();
builder.Services.AddHostedService<MarketEvaluationService>();

builder.Services.AddSingleton(provider => {
    var factory = provider.GetRequiredService<IIndicatorFactory>();
    return new List<TradeProfile>() { new HighRsiProfile(factory), new ScalpingTradeProfile(factory) };
});

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
app.UseResponseCaching();
app.UseAuthentication(); // Make sure this comes before UseAuthorization()
app.UseAuthorization();

app.MapControllers().RequireAuthorization();
app.MapFallbackToFile("index.html");
app.Run();