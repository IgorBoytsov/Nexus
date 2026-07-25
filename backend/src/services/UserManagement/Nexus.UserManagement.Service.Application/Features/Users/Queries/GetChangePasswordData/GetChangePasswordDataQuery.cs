using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Contracts.UserManagement.Responses;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetChangePasswordData
{
    public sealed record GetChangePasswordDataQuery(Guid UserId) : IRequest<Result<GetChangePasswordDataResponse>>, IHasGuidUserId, IQuery;
}