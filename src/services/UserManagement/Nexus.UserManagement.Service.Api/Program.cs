using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RegisterAdmin;
using Nexus.UserManagement.Service.Application.Ioc;
using Nexus.UserManagement.Service.Infrastructure.Ioc;
using System.Text;

namespace Nexus.UserManagement.Service.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMvcApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5131")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddInfrastructure(builder.Configuration).AddApplication();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
                    )
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (args.FirstOrDefault()?.ToLower() == "create-admin")
            {
                Console.WriteLine("Запуск команды создания администратора...");

                string? GetArgumentValue(string argName)
                {
                    var argIndex = Array.FindIndex(args, a => a.Equals(argName, StringComparison.OrdinalIgnoreCase));

                    if (argIndex != -1 && argIndex + 1 < args.Length)
                        return args[argIndex + 1];
                    return null;
                }

                var username = GetArgumentValue("--username");
                var email = GetArgumentValue("--email");
                var password = GetArgumentValue("--password");

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка: не все обязательные аргументы переданы.");
                    Console.WriteLine("Пример использования: dotnet run create-admin --username <имя> --email <email> --password <пароль>");
                    Console.ResetColor();
                    return;
                }

                using var scope = app.Services.CreateScope();

                try
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var command = new RegisterAdminCommand(username, username, password, email, Phone: null, IdGender: null, IdCountry: null);
                    var result = await mediator.Send(command);

                    if (result.IsSuccess)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Администратор '{username}' успешно создан");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ошибка при создании администратора: {result.StringMessage}");
                    }
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Произошла критическая ошибка: {ex.Message}");
                    Console.ResetColor();
                }

                return;
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowMvcApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}