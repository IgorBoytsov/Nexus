namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record UpdateStatusRequest(Guid Id, string Name);
}