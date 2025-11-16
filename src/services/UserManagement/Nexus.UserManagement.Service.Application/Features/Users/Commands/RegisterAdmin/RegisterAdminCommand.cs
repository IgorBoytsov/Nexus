using MediatR;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RegisterAdmin
{
    public sealed record RegisterAdminCommand(
        string Login, string UserName,
        string Password,
        string Email, string? Phone,
        Guid? IdGender, Guid? IdCountry) : IRequest<Result>;
}