using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Delete
{
    public sealed class DeleteAccountCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteAccountCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybe = await repository.GetByAsync(u => u.Id == request.UserId);

            if (maybe.IsNone)
                return new Error(ErrorCode.NotFound, "Пользователь не найден");

            User user = maybe.Value;

            user.Delete();

            repository.Remove(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}