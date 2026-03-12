using MediatR;
using Quantropic.Toolkit.Results;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Authentication.Service.Application.Features.Commands.LoginByToken
{
    public record TokenLoginCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
}