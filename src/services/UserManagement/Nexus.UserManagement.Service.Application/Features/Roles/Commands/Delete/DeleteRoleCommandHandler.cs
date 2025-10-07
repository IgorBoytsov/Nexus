using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Delete
{
    public sealed class DeleteRoleCommandHandler(IWriteDbContext writeContext) : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _writeContext.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

                if (role == null)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                _writeContext.Roles.Remove(role);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Delete, "Ошибка на стороне сервера"));
            }
        }
    }
}