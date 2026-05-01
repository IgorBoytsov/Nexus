using MediatR;
using Crossdyne.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo
{
    public sealed record GetProfileInfoQuery(Guid UserId) : IRequest<Result<ProfileInfoResponse>>;
}