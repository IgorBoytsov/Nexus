using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Delete
{
    public sealed class DeleteRoleCommandHandler(
        IUnitOfWork unitOfWork, 
        IRoleRepository roleRepository) : IRequestHandler<DeleteRoleCommand, Result>
    {
        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<Role> maybeRole = await roleRepository.GetByAsync(r => r.Id == request.Id, cancellationToken);

                if (maybeRole.IsNone)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                Role role = maybeRole.Value;

                roleRepository.Remove(role);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Delete, "Ошибка на стороне сервера"));
            }
        }
    }
}