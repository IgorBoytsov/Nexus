using System.Reflection;
using Nexus.Bff.Extensions;
using Shared.Validations.Extensions;

var builder = WebApplication.CreateBuilder(args);

var executingAssembly = Assembly.GetExecutingAssembly(); 
var configuration = builder.Configuration;
var environment = builder.Environment; 

builder.Services
    //Default
    .AddOpenApi()
    .AddAuthorization()
    // Custom
    .ConfigureOptions()
    .AddServices(configuration)
    .AddHttpClients(configuration)
    .AddValidations(executingAssembly)
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
app.UseCors("AllowLocalFrontend");
app.UseAuthentication(); 
app.UseAuthorization();  
app.MapEndpoints(executingAssembly);
app.Run();