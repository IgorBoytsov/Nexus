using System.Reflection;
using Nexus.Bff.Extensions;
using Serilog;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

var executingAssembly = Assembly.GetExecutingAssembly(); 
var configuration = builder.Configuration;
var environment = builder.Environment; 
var host = builder.Host;

builder.Logging.ClearProviders();
builder.Host.AddSerilogLogger(); 

builder.Services
    //Default
    .AddOpenApi()
    .AddAuthorization()
    // Custom
    .ConfigureOptions()
    .AddServices(configuration)
    .AddHttpClients(configuration)
    .AddDistributedLock()
    .UseCors()
    .AddSharedCryptoKeyForDecryptCookie(configuration)
    .AddCookie(environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSerilogRequestLogging();
app.UseCors("AllowLocalFrontend");
app.UseAuthentication(); 
app.UseAuthorization();  
app.MapEndpoints(executingAssembly);
app.Run();