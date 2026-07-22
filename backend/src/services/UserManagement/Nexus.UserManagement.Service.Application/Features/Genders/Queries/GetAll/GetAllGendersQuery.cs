using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Queries.GetAll
{
    public sealed record GetAllGendersQuery() : IRequest<List<GenderResponse>>, IQuery;
}