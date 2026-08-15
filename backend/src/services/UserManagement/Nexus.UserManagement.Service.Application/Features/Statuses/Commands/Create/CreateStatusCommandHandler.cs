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
            Status status = Status.Create(request.Name);

            await statusRepository.AddAsync(status, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return status.Id;
        }
    }
}