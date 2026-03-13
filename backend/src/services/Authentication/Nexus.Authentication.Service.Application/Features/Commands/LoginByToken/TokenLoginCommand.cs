using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.V1;

namespace Nexus.Authentication.Service.Application.Features.Commands.LoginByToken
{
    public record TokenLoginCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
}