using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Create
{
    public sealed class CreateRoleCommandHandler(
        IUnitOfWork unitOfWork, 
        IRoleRepository roleRepository) : IRequestHandler<CreateRoleCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = Role.Create(request.Name);

            await roleRepository.AddAsync(role, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return role.Id.Value;
        }
    }
}