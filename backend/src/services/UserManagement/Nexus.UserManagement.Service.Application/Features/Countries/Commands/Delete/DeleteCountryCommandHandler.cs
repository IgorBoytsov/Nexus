using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Delete
{
    public sealed class DeleteCountryCommandHandler(
        IUnitOfWork unitOfWork, 
        ICountryRepository countryRepository) : IRequestHandler<DeleteCountryCommand, Result>
    {
        public async Task<Result> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<Country> maybeCountry = await countryRepository.GetByAsync(c => c.Id == request.Id, cancellationToken);

                if (maybeCountry.IsNone)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                Country country = maybeCountry.Value;

                countryRepository.Remove(country);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Delete, "Ошибка на стороне сервера"));
            }
        }
    }
}