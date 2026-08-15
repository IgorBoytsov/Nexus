using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Contracts.Cache.Abstractions;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeEmailSendCode
{
    public sealed class ChangeEmailSendCodeCommandHandler(
        IUserRepository repository, 
        IUnitOfWork unitOfWork,
        ICacheService redisCacheService) : IRequestHandler<ChangeEmailSendCodeCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(ChangeEmailSendCodeCommand request, CancellationToken cancellationToken)
        {
            Email email = Email.Create(request.Email);

            if (await repository.CheckAvailableEmail(email))
                return new Error(ErrorCode.Conflict, "На данный Email уже зарегистрирована учетная запись.");

            Maybe<User> maybe = await repository.GetByAsync(u => u.Id == request.UserId);

            if (maybe.IsNone)
                return new Error(ErrorCode.NotFound, "Пользователь не найден");

            User user = maybe.Value;

            var code = user.GetChangeEmailCode(email);

            await redisCacheService.SetStringAsync($"ConfirmCode for {user.Login.Value.ToLowerInvariant()}", code, TimeSpan.FromMinutes(10));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}