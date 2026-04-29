using System.IdentityModel.Tokens.Jwt;

namespace Nexus.Bff.Services
{
    public record JwtExtractedData(string UserId, string Login);

    public sealed class JwtReadService
    {
        private static readonly JwtSecurityTokenHandler _handler = new();

        public JwtExtractedData ExtractData(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var login = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(login))
                throw new InvalidOperationException("Access token is missing required claims (sub/name).");

            return new JwtExtractedData(userId, login);
        }
    }
}