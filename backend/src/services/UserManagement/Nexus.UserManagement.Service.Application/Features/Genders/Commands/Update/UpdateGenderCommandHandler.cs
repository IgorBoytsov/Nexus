using MediatR;
using Nexus.UserManagement.Service.Domain.ValueObjects.Gender;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Update
{
    public sealed class UpdateGenderCommandHandler(
        IUnitOfWork unitOfWork, 
        IGenderRepository genderRepository) : IRequestHandler<UpdateGenderCommand, Result>
    {
        public async Task<Result> Handle(UpdateGenderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<Gender> maybeGender = await genderRepository.GetByAsync(g => g.Id == request.Id, cancellationToken);

                if (maybeGender.IsNone)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                Gender gender = maybeGender.Value;

                gender.UpdateName(GenderName.Create(request.Name));

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