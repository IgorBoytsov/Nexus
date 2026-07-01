namespace Shared.Contracts.UserManagement.Responses
{
    public sealed record ProfileInfoResponse(
        string Login, 
        string UserName,
        string Email, 
        DateTime DateRegistration,
        S3KeyResponse? AvatarS3Key);
}