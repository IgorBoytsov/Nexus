using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Nexus.Authentication.Service.Application.Extensions;
using Nexus.Authentication.Service.Infrastructure.Extensions;
using Serilog;
using Shared.Logging;
using Shared.Web.Extensions;

namespace Nexus.Authentication.Service.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.Configure<JsonSerializerOptions>(opt => opt.AddCrossdyneDefaults());
            
            builder.Host.AddSerilogLogger();
            
            IConfiguration configuration = builder.Configuration;

            //Api
            builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.AddCrossdyneDefaults());
            builder.Services.AddOpenApi();

            //Application
            builder.Services
                .RegisterMediator()
                .RegisterSecurity()
                .RegisterValidation();

            //Infrastructure
            builder.Services
                .RegisterDatabase(configuration)
                .RegisterRepositories()
                .RegisterHttpClients(configuration)
                .RegisterBackgroundServices()
                .RegisterMessaging(configuration)
                .RegisterCache(configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();
            app.MapControllers();

            app.Run();
        }
    }
}