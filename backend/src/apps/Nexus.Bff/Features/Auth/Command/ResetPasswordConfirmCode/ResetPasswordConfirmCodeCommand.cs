using MediatR;
using Crossdyne.Toolkit.Results;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
{
    public sealed record ResetPasswordConfirmCodeCommand(string Login, string Code) : IRequest<Result>; 
}