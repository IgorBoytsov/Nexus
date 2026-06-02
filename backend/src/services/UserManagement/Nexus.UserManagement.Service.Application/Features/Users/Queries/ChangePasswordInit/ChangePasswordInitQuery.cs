using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ChangePasswordInit
{
    public sealed record ChangePasswordInitQuery(Guid UserId) : IRequest<Result<ChangePasswordInitResponse>>, IHasGuidUserId;
}