using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create
{
    public sealed class CreateCountryCommandHandler(IWriteDbContext writeContext) : IRequestHandler<CreateCountryCommand, Result<Guid>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<Guid>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var country = Country.Create(request.Name);

                await _writeContext.Countries.AddAsync(country, cancellationToken);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(country.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}