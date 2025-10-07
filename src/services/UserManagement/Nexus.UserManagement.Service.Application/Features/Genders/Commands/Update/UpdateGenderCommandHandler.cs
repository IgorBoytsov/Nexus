using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.Gender;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Update
{
    public sealed class UpdateGenderCommandHandler(IWriteDbContext writeContext) : IRequestHandler<UpdateGenderCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(UpdateGenderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var gender = await _writeContext.Genders.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (gender == null)
                    return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

                gender.UpdateName(GenderName.Create(request.Name));

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