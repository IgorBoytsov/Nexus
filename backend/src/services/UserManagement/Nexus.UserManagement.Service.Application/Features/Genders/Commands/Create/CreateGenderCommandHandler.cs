using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create
{
    public sealed class CreateGenderCommandHandler(
        IUnitOfWork unitOfWork, 
        IGenderRepository genderRepository) : IRequestHandler<CreateGenderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateGenderCommand request, CancellationToken cancellationToken)
        {
            var gender = Gender.Create(request.Name);

            await genderRepository.AddAsync(gender, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return gender.Id;
        }
    }
}