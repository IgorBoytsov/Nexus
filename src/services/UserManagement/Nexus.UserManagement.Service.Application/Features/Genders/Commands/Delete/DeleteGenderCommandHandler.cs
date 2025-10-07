using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Delete
{
    public sealed class DeleteGenderCommandHandler(IWriteDbContext writeContext) : IRequestHandler<DeleteGenderCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(DeleteGenderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var gender = await _writeContext.Genders.FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

                if (gender == null)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                _writeContext.Genders.Remove(gender);

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