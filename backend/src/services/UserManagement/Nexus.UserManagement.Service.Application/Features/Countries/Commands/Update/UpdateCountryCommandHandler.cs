using MediatR;
using Nexus.UserManagement.Service.Domain.ValueObjects.Country;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Update
{
    public sealed class UpdateCountryCommandHandler(
        IUnitOfWork unitOfWork, 
        ICountryRepository countryRepository) : IRequestHandler<UpdateCountryCommand, Result>
    {
        public async Task<Result> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<Country> maybeCountry = await countryRepository.GetByAsync(c => c.Id == request.Id, cancellationToken);

                if (maybeCountry.IsNone)
                    return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

                Country country = maybeCountry.Value;

                country.UpdateName(CountryName.Create(request.Name));

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