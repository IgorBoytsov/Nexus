using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.ResetPasswordConfirmCode
{
    public sealed record ResetPasswordConfirmCodeCommand(string Login, string Code) : IRequest<Result>, IHasLogin, IHasCode; 
}