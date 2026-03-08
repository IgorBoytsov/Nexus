namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record UpdateRoleRequest(Guid Id, string Name);
}