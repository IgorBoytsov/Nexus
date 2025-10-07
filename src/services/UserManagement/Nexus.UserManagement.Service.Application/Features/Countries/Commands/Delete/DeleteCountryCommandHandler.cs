using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Delete
{
    public sealed class DeleteCountryCommandHandler(IWriteDbContext writeContext) : IRequestHandler<DeleteCountryCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _writeContext.Countries.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (country == null)
                    return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

                _writeContext.Countries.Remove(country);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Delete, "Ошибка на стороне сервера"));
            }
        }
    }
}