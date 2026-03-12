using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryAccess
{
    public sealed record RecoveryAccessCommand(string Login, string Email, string NewPassword) : IRequest<Result>;
}