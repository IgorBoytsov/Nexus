using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ExistByLogin
{
    public sealed record class ExistUserByLoginQuery(string Login) : IRequest<Result>, IHasLogin;
}