namespace Nexus.Bff.Features.Auth.Command.CheckAuthStatus
{
    public static class CheckAuthStatusEndpoint
    {
        public static void MapCheckAuthStatus(this IEndpointRouteBuilder app)
            => app.MapGet("auth/status", () => Results.Ok(new { isAuthenticated = true })).RequireAuthorization();
    }
}