using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.Bff.Features.Auth.Query.GetRecoveryKeys
{
    public sealed record GetRecoveryKeysQuery(string Login) : IRequest<Result<RecoveryViaKeysPayloadResponse>>, IHasLogin;
}