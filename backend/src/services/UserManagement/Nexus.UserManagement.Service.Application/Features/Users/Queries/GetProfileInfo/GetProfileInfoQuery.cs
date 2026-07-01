using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo
{
    public sealed record GetProfileInfoQuery(Guid UserId) : IRequest<Result<ProfileInfoResponse>>;
}