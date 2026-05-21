using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.RecoveryViaKeysInit
{
    public sealed record RecoveryViaKeysInitCommand(string Login) : IRequest<Result<RecoveryViaKeysPayloadResponse>>, IHasLogin;
}