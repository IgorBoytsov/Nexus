using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Delete
{
    public sealed class DeleteStatusCommandHandler(
        IUnitOfWork unitOfWork, 
        IStatusRepository statusRepository) : IRequestHandler<DeleteStatusCommand, Result>
    {
        public async Task<Result> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
        {
            Maybe<Status> maybeStatus = await statusRepository.GetByAsync(r => r.Id == request.Id, cancellationToken);

            if (maybeStatus.IsNone)
                return Result.Failure(new Error(ErrorCode.Delete, "Такой записи не существует."));

            Status status = maybeStatus.Value;

            statusRepository.Remove(status);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}