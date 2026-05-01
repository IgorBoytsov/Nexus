using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordSendCode
{
    public sealed record ResetPasswordSendCodeCommand(string Login) : IRequest<Result>;
}