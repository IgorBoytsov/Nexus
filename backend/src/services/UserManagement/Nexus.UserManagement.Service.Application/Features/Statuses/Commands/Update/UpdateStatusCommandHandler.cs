using MediatR;
using Nexus.UserManagement.Service.Domain.ValueObjects.Status;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update
{
    public sealed class UpdateStatusCommandHandler(
        IUnitOfWork unitOfWork, 
        IStatusRepository statusRepository) : IRequestHandler<UpdateStatusCommand, Result>
    {
        public async Task<Result> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<Status> maybeStatus = await statusRepository.GetByAsync(r => r.Id == request.Id, cancellationToken);

                if (maybeStatus.IsNone)
                    return Result.Failure(new Error(ErrorCode.Update, "Такой записи не существует."));

                Status status = maybeStatus.Value;

                status.UpdateName(StatusName.Create(request.Name));

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