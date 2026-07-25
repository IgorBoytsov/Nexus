using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Responses;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo
{
    public sealed record GetProfileInfoQuery(Guid UserId) : IRequest<Result<ProfileInfoResponse>>, IQuery;
}