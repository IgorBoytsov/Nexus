using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Nexus.Authentication.Service.Application.Extensions;
using Nexus.Authentication.Service.Infrastructure.Extensions;
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

            builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.AddCrossdyneDefaults());
            
            builder.Services.AddOpenApi();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}