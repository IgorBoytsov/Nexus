using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Update
{
    public sealed class UpdateRoleCommandHandler(IWriteDbContext writeContext) : IRequestHandler<UpdateRoleCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _writeContext.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

                if (role == null)
                    return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

                role.UpdateName(RoleName.Create(request.Name));

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Update, "Ошибка на стороне сервера"));
            }
        }
    }
}