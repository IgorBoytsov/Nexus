using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetDek
{
    public sealed record GetDekQuery(Guid UserId) : IRequest<Result<DekResponse>>, IQuery;
}