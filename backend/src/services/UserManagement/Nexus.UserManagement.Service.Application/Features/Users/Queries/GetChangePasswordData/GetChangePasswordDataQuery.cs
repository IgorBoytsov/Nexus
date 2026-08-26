using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Abstractions.Validations;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetChangePasswordData
{
    public sealed record GetChangePasswordDataQuery(Guid UserId) : IRequest<Result<GetChangePasswordDataResponse>>, IHasGuidUserId, IQuery;
}