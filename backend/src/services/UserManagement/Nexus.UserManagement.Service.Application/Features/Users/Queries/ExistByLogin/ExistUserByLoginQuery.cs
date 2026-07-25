using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ExistByLogin
{
    public sealed record class ExistUserByLoginQuery(string Login) : IRequest<Result>, IHasLogin, IQuery;
}