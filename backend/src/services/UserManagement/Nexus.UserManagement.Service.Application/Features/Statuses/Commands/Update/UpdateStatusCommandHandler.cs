using MediatR;
using Nexus.UserManagement.Service.Domain.ValueObjects.Status;
using Crossdyne.Toolkit.Results;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update
{
    public sealed class UpdateStatusCommandHandler(
        IUnitOfWork unitOfWork, 
        IStatusRepository statusRepository) : IRequestHandler<UpdateStatusCommand, Result>
    {
        public async Task<Result> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            Maybe<Status> maybeStatus = await statusRepository.GetByAsync(r => r.Id == request.Id, cancellationToken);

            if (maybeStatus.IsNone)
                return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

            Status status = maybeStatus.Value;

            status.UpdateName(StatusName.Create(request.Name));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}