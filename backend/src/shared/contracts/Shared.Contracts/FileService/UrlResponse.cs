namespace Shared.Contracts.FileService
{
    public record UrlResponse(string Status, string Url, int ExpiresIn);
}