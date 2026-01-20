namespace Shared.Contracts.UserMenagement.Requests
{
    public sealed record UpdateRoleRequest(Guid Id, string Name);
}