namespace Shared.Contracts.UserMenagement.Requests
{
    public sealed record UpdateStatusRequest(Guid Id, string Name);
}