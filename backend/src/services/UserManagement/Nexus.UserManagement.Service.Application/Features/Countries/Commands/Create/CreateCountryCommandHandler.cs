using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create
{
    public sealed class CreateCountryCommandHandler(
        IUnitOfWork unitOfWork, 
        ICountryRepository countryRepository) : IRequestHandler<CreateCountryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var country = Country.Create(request.Name);

                await countryRepository.AddAsync(country, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(country.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}