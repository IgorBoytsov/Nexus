using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysInit
{
    public sealed record RecoveryViaKeysInitCommand(string Login) : IRequest<Result<RecoveryViaKeysPayloadResponse>>, IHasLogin;
}