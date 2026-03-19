using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserAuthDataResponse>>;
}