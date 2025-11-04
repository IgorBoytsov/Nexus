using MediatR;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Login
{
    public sealed record LoginUserCommand(string Password, string Login) : IRequest<Result<UserDto>>;
}