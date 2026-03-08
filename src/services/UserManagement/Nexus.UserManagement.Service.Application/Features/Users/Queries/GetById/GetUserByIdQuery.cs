using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserAuthDataResponse>>;
}