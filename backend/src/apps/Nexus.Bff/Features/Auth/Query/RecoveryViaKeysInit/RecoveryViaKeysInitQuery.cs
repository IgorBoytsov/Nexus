using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Query.RecoveryViaKeysInit
{
    public sealed record RecoveryViaKeysInitQuery(string Login) : IRequest<Result<RecoveryViaKeysPayloadResponse>>, IHasLogin;
}