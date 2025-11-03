using Nexus.Authentication.Service.Application.Features.Commands.Login;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Infrastructure.Ioc;
using Shared.Security.Hasher;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

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

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            builder.Services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            builder.Services.AddHttpClient<IUserManagementServiceClient, UserManagementServiceClient>(client => client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:UserManagement"]!));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

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