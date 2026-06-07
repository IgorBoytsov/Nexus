namespace Nexus.Bff.Services
{
    public interface IJwtReadService
    {
        JwtExtractedData ExtractData(string token);
    }
}