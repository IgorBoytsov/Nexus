using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Infrastructure.Ioc;
using System.IdentityModel.Tokens.Jwt;
using Nexus.Authentication.Service.Application.Ioc;

namespace Nexus.Authentication.Service.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddHttpClient<IUserManagementServiceClient, UserManagementServiceClient>(client => client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:UserManagement"]!));

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