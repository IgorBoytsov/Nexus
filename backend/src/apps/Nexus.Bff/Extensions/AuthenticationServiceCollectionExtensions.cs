using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Nexus.Bff.Infrastructure.Clients;
using Nexus.Bff.Services;
using Shared.Contracts;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Common;
using StackExchange.Redis;

namespace Nexus.Bff.Extensions
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedCryptoKeyForDecryptCookie(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration["Redis:ConnectionString"];
            var redis = ConnectionMultiplexer.Connect(redisConnectionString!);

            services.AddDataProtection()
                .SetApplicationName("Crossdyne.SharedBff")
                .PersistKeysToStackExchangeRedis(redis, RedisKeyExtensions.DataProtectionKeys());

            return services;
        }

        public static IServiceCollection UseCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalFrontend", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:4200", "https://account.crossdyne.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddCookie(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "Crossdyne";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;

                if (environment.IsDevelopment())
                {
                    options.Cookie.Domain = "127.0.0.1"; 
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
                    var cacheSessionKey = RedisKeyExtensions.SessionKey(sessionId!);

                    if (string.IsNullOrWhiteSpace(sessionId))
                    {
                        context.RejectPrincipal();
                        return;
                    }

                    var cache = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheService>();
                    var session = await cache.GetJsonAsync<UserSession>(cacheSessionKey);
                
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

                            await cache.SetJsonAsync(cacheSessionKey, session, TimeSpan.FromDays(30));
                        }
                        else
                        {
                            var updatedSession = await cache.GetJsonAsync<UserSession>(cacheSessionKey);

                            if (updatedSession != null && updatedSession.AccessTokenExpiresAt > DateTime.UtcNow.AddMinutes(1))
                            {
                                session = updatedSession;
                            }
                            else
                            {
                                await cache.RemoveAsync(cacheSessionKey);
                                context.RejectPrincipal();
                                return;
                            }
                        }
                    }

                    context.HttpContext.Items["AccessToken"] = session.AccessToken;
                };
            });

            return services;
        }
    }
}