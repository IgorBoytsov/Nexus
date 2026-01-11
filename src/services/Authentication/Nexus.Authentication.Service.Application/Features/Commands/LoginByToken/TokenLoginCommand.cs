using MediatR;
using Shared.Contracts.Responses;
using Shared.Kernel.Results;

namespace Nexus.Authentication.Service.Application.Features.Commands.LoginByToken
{
    public record TokenLoginCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
}