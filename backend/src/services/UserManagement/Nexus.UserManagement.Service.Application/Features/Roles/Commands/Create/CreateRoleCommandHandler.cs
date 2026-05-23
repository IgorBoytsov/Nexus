using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Create
{
    public sealed class CreateRoleCommandHandler(
        IUnitOfWork unitOfWork, 
        IRoleRepository roleRepository) : IRequestHandler<CreateRoleCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = Role.Create(request.Name);

                await roleRepository.AddAsync(role, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(role.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}