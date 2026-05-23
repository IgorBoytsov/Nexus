using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create
{
    public sealed class CreateGenderCommandHandler(
        IUnitOfWork unitOfWork, 
        IGenderRepository genderRepository) : IRequestHandler<CreateGenderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateGenderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var gender = Gender.Create(request.Name);

                await genderRepository.AddAsync(gender, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(gender.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}