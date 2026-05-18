using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserAuthDataResponse>>;
}