using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode
{
    public sealed record ResetPasswordSendCodeCommand(string Login) : IRequest<Result>, IHasLogin;
}