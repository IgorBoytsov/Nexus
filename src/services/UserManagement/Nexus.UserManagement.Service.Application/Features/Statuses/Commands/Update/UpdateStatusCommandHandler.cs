using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.Status;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update
{
    public sealed class UpdateStatusCommandHandler(IWriteDbContext writeContext) : IRequestHandler<UpdateStatusCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var status = await _writeContext.Statuses.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

                if (status == null)
                    return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

                status.UpdateName(StatusName.Create(request.Name));

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