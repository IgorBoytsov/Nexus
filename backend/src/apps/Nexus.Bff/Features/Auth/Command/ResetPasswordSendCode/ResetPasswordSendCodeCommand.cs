using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordSendCode
{
    public sealed record ResetPasswordSendCodeCommand(string Login) : IRequest<Result>, IHasLogin;
}