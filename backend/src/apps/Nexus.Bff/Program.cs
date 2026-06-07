using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Nexus.Bff.Extensions;
using Nexus.Bff.Infrastructure.Clients;
using Nexus.Bff.Services;
using Shared.Contracts;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Common;
using Shared.Validations.Extensions;
using Shared.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assembly = Assembly.GetExecutingAssembly(); 

builder.Services.AddOpenApi();

builder.Services.Configure<JsonSerializerOptions>(opt => opt.AddCrossdyneDefaults());

builder.Services.AddAuthorization();
builder.Services.AddServices(builder.Configuration).AddHttpClients(builder.Configuration).AddValidations(assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:4200", "https://account.crossdyne.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Crossdyne";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;

        if (builder.Environment.IsDevelopment())
        {
            options.Cookie.Domain = null;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
            options.Cookie.SameSite = SameSiteMode.Lax;
        }
        else
        {
            options.Cookie.Domain = ".crossdyne.com"; 
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        }

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };

        options.Events.OnValidatePrincipal = async context =>
        {
            var sessionId = context.Principal?.FindFirst("SessionId")?.Value;

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                context.RejectPrincipal();
                return;
            }

            var cache = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheService>();
            var session = await cache.GetJsonAsync<UserSession>(sessionId);
           
            if (session == null)
            {
                context.RejectPrincipal();
                return;
            }

            if (session.AccessTokenExpiresAt <= DateTime.UtcNow.AddMinutes(1))
            {
                var authClient = context.HttpContext.RequestServices.GetRequiredService<IAuthClient>();

                var refreshResult = await authClient.RefreshTokens(new RefreshTokensRequest(session.RefreshToken, session.AccessToken));

                if (refreshResult.IsSuccess)
                {
                    var jwtReader = context.HttpContext.RequestServices.GetRequiredService<IJwtReadService>();
                    var jwtData = jwtReader.ExtractData(refreshResult.Value.AccessToken);

                    session.AccessToken = refreshResult.Value.AccessToken;
                    session.RefreshToken = refreshResult.Value.RefreshToken;
                    session.AccessTokenExpiresAt = jwtData.ExpiredTime;

                    await cache.SetJsonAsync(sessionId, session, TimeSpan.FromDays(30));
                }
                else
                {
                    var updatedSession = await cache.GetJsonAsync<UserSession>(sessionId);

                    if (updatedSession != null && updatedSession.AccessTokenExpiresAt > DateTime.UtcNow.AddMinutes(1))
                    {
                        session = updatedSession;
                    }
                    else
                    {
                        await cache.RemoveAsync(sessionId);
                        context.RejectPrincipal();
                        return;
                    }
                }
            }

            context.HttpContext.Items["AccessToken"] = session.AccessToken;
            context.ShouldRenew = true;
        };
    });

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
app.MapEndpoints(Assembly.GetExecutingAssembly());
app.Run();