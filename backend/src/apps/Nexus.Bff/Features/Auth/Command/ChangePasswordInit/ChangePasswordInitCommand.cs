using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.ChangePasswordInit
{
    public sealed record ChangePasswordInitCommand(Guid UserId) : IRequest<Result<ChangePasswordInitResponse>>, IHasGuidUserId;
}