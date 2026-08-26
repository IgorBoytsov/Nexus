using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create
{ 
    public sealed class CreateCountryCommandHandler( 
        IUnitOfWork unitOfWork, 
        ICountryRepository countryRepository) : IRequestHandler<CreateCountryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = Country.Create(request.Name);

            await countryRepository.AddAsync(country, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return country.Id;
        }
    }
}