using MediatR;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetByLoginInternal
{
    public sealed record GetUserByLoginInternalQuery(string Login) : IRequest<Result<UserAuthDataDto>>;
}