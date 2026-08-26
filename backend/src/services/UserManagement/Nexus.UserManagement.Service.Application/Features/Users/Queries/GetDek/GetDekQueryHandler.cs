using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetDek
{
    public sealed class GetDekQueryHandler(IUserReadOnlyRepository repository) : IRequestHandler<GetDekQuery, Result<DekResponse>>
    {
        public async Task<Result<DekResponse>> Handle(GetDekQuery request, CancellationToken cancellationToken)
            => await repository.GetDek(request.UserId);
    }
}