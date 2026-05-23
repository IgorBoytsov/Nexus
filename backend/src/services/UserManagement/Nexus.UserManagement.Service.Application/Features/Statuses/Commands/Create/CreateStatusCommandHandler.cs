using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Create
{
    public sealed class CreateStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IStatusRepository statusRepository) : IRequestHandler<CreateStatusCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Status status = Status.Create(request.Name);

                await statusRepository.AddAsync(status, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(status.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}