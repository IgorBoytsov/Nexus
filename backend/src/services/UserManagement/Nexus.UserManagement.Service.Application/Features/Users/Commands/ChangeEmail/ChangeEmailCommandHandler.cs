using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Contracts.Cache.Abstractions;
using Shared.Kernel.Errors;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeEmail
{
    public sealed class ChangeEmailCommandHandler(
        IUserRepository repository, 
        IUnitOfWork unitOfWork,
        ICacheService cache) : IRequestHandler<ChangeEmailCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybe = await repository.GetByAsync(u => u.Id == request.UserId, includes: [u => u.UserAuthenticators], clt: cancellationToken);

            if (maybe.IsNone)
                return new Error(ErrorCode.NotFound, "Пользователь не найден");

            User user = maybe.Value;

            Email email = Email.Create(request.Email);

            var storageVerifyCode = await cache.GetStringAsync($"ConfirmCode for {user.Login.Value.ToLowerInvariant()}");

            if (storageVerifyCode != request.Code)
                return new Error(AppErrors.CodeVerifier, "Вы ввели не верный код подтверждения");

            user.ChangeEmail(email);
            user.ChangeEmailAuthenticator(email);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}