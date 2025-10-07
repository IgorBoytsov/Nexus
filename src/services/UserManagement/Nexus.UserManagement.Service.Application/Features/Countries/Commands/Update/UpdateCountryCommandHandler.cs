using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.Country;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Update
{
    public sealed class UpdateCountryCommandHandler(IWriteDbContext writeContext) : IRequestHandler<UpdateCountryCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _writeContext.Countries.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (country == null)
                    return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

                country.UpdateName(CountryName.Create(request.Name));

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