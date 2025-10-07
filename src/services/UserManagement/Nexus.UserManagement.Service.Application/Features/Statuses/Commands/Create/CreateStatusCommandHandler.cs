using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Create
{
    public sealed class CreateStatusCommandHandler(IWriteDbContext writeContext) : IRequestHandler<CreateStatusCommand, Result<Guid>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<Guid>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var status = Status.Create(request.Name);

                await _writeContext.Statuses.AddAsync(status, cancellationToken);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(status.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}