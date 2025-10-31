using MediatR;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.Login
{
    public sealed record LoginCommand(string Login, string Password) : IRequest<Result<AuthResponse>>;
}
