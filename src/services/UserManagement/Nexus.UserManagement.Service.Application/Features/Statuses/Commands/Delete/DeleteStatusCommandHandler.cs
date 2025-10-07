using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Delete
{
    public sealed class DeleteStatusCommandHandler(IWriteDbContext writeContext) : IRequestHandler<DeleteStatusCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _writeContext.Statuses.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

                if (status == null)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                _writeContext.Statuses.Remove(status);

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