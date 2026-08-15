using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Domain.Models;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.Authentication.Service.Application.Features.Commands.Logout
{
    public sealed class LogoutCommandHandler(IAccessDataRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            Maybe<AccessData> maybe = await repository.GetByAsync(x => x.RefreshTokenHash == request.RefreshToken);

            if (maybe.IsNone)
                return Unit.Value;

            AccessData accessData = maybe.Value;

            repository.Remove(accessData);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}