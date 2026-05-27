using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Query.ChangePasswordInit
{
    public sealed record ChangePasswordInitQuery(Guid UserId) : IRequest<Result<ChangePasswordInitResponse>>, IHasGuidUserId;
}