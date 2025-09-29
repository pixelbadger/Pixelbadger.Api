using Microsoft.Identity.Web;
using Pixelbadger.Api.Application.Queries;
using Pixelbadger.Api.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Azure AD authentication
builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetWeatherForecastHandler).Assembly));

// Add Microsoft Graph SharePoint service
var graphConfig = builder.Configuration.GetSection("MicrosoftGraph");
builder.Services.AddSingleton<ISharePointService>(sp =>
    new SharePointService(
        graphConfig["TenantId"] ?? throw new InvalidOperationException("MicrosoftGraph:TenantId is required"),
        graphConfig["ClientId"] ?? throw new InvalidOperationException("MicrosoftGraph:ClientId is required"),
        graphConfig["ClientSecret"] ?? throw new InvalidOperationException("MicrosoftGraph:ClientSecret is required")
    ));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program accessible for testing
public partial class Program { }
