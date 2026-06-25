using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.Bff.Features.Profile.Query.GetChangePasswordData
{
    public sealed record GetChangePasswordDataQuery(Guid UserId) : IRequest<Result<GetChangePasswordDataResponse>>, IHasGuidUserId;
}