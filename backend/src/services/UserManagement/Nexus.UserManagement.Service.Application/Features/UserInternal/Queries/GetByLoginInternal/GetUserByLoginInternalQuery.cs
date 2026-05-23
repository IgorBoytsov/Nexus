using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts;

namespace Nexus.UserManagement.Service.Application.Features.UserInternal.Queries.GetByLoginInternal
{
    public sealed record GetUserByLoginInternalQuery(string Login) : IRequest<Result<UserAuthDataResponse>>;
}