using MediatR;
using Shared.Contracts.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetByLoginInternal
{
    public sealed record GetUserByLoginInternalQuery(string Login) : IRequest<Result<UserAuthDataDto>>;
}