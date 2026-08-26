using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Delete
{
    public sealed class DeleteGenderCommandHandler(
        IUnitOfWork unitOfWork, 
        IGenderRepository genderRepository) : IRequestHandler<DeleteGenderCommand, Result>
    {
        public async Task<Result> Handle(DeleteGenderCommand request, CancellationToken cancellationToken)
        {
            Maybe<Gender> maybeGender = await genderRepository.GetByAsync(g => g.Id == request.Id, cancellationToken);

            if (maybeGender.IsNone)
                return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

            Gender gender = maybeGender.Value;

            genderRepository.Remove(gender);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}