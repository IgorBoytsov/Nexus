using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetRecoveryKeys
{
    public sealed record GetRecoveryKeysQuery(string Login) : IRequest<Result<RecoveryViaKeysPayloadResponse>>, IHasLogin;
}