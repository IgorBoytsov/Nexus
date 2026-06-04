using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetRecoveryKeys
{
    public sealed record GetRecoveryKeysQuery(string Login) : IRequest<Result<RecoveryViaKeysPayloadResponse>>, IHasLogin;
}