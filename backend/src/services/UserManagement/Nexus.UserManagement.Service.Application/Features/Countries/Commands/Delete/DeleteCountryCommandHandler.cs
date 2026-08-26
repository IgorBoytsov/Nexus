using MediatR;
using Crossdyne.Toolkit.Results;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Delete
{
    public sealed class DeleteCountryCommandHandler(
        IUnitOfWork unitOfWork, 
        ICountryRepository countryRepository) : IRequestHandler<DeleteCountryCommand, Result>
    {
        public async Task<Result> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            Maybe<Country> maybeCountry = await countryRepository.GetByAsync(c => c.Id == request.Id, cancellationToken);

            if (maybeCountry.IsNone)
                return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

            Country country = maybeCountry.Value;

            countryRepository.Remove(country);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}