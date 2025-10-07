namespace Nexus.UserManagement.Service.Api.Models.Requests
{
    public sealed record class RegisterUserRequest(
        string Login, string UserName,
        string Password,
        string Email, string? Phone,
        Guid? IdGender, Guid? IdCountry);
}