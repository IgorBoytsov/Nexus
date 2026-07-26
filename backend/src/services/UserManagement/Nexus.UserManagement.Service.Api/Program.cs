using Nexus.UserManagement.Service.Api.Extensions;
using Nexus.UserManagement.Service.Application.Extension;
using Nexus.UserManagement.Service.Infrastructure.Extension;
using Serilog;
using Shared.Logging;
using Shared.Web.Extensions;

namespace Nexus.UserManagement.Service.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = builder.Configuration;
            string databaseConnectionString = configuration.GetConnectionString("DefaultConnection")!;

            builder.Host.AddSerilogLogger();

            //Api
            builder.Services
                .RegisterAuthentication(configuration)
                .RegisterCors();

            builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.AddCrossdyneDefaults());

            //Application
            builder.Services
                .RegisterMediator()
                .RegisterValidation()
                .RegisterMapping(configuration);

            // Infrastructure
            builder.Services
                .RegisterCache(configuration)
                .RegisterMessaging(configuration)
                .RegisterWriteDatabase(configuration, databaseConnectionString)
                .RegisterReadonlyDatabase(databaseConnectionString)
                .RegisterRepositories()
                .RegisterHttpClients(configuration);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowMvcApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();

            app.UseSerilogRequestLogging();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}