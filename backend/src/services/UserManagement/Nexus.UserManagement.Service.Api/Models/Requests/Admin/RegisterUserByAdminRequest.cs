namespace Nexus.UserManagement.Service.Api.Models.Requests.Admin
{
    public sealed record RegisterUserByAdminRequest(
        string Login, string UserName,
        string Password,
        string Email, string? Phone,
        Guid? IdGender, Guid? IdCountry);
}
