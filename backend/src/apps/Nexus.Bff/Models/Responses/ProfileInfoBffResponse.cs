namespace Nexus.Bff.Models.Responses
{
    public sealed record ProfileInfoBffResponse(string Login, 
        string UserName,
        string Email, 
        DateTime DateRegistration,
        string AvatarUrl);
}