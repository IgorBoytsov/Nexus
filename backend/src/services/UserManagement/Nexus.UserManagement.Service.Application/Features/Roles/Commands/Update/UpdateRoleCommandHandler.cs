using MediatR;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Primitives;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Update
{
    public sealed class UpdateRoleCommandHandler(
        IUnitOfWork unitOfWork, 
        IRoleRepository roleRepository) : IRequestHandler<UpdateRoleCommand, Result>
    {
        public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<Role> maybeRole = await roleRepository.GetByAsync(r => r.Id == request.Id, cancellationToken);

                if (maybeRole.IsNone)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                Role role = maybeRole.Value;

                role.UpdateName(RoleName.Create(request.Name));

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Update, "Ошибка на стороне сервера"));
            }
        }
    }
}