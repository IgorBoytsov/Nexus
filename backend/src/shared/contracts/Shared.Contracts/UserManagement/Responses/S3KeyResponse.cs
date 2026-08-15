namespace Shared.Contracts.UserManagement.Responses
{
    public sealed record S3KeyResponse(string Key, string Bucket, string FolderPath);
}