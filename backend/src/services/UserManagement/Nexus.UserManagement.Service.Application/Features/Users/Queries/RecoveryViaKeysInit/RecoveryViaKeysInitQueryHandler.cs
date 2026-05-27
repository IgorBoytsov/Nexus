using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Contracts;
using Shared.Kernel.Errors;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitQueryHandler(
        IUserRepository userRepository) : IRequestHandler<RecoveryViaKeysInitQuery, Result<RecoveryViaKeysPayloadResponse>>
    {
        public async Task<Result<RecoveryViaKeysPayloadResponse>> Handle(RecoveryViaKeysInitQuery request, CancellationToken cancellationToken)
        {
            Maybe<User> maybeUser = await userRepository.GetByAsync(u => u.Login == request.Login, includes: x => x.RecoveryKeys, clt: cancellationToken);

            if (maybeUser.IsNone)
                return Result<RecoveryViaKeysPayloadResponse>.Failure(new Error(ErrorCode.NotFound, "Пользователя с таким логином не существует."));

            User user = maybeUser.Value;

            var recoveryKeys = user.RecoveryKeys;

            if (!recoveryKeys.Any())
                return Result<RecoveryViaKeysPayloadResponse>.Failure(new Error(AppErrors.AccountNotSetUpForRecovery, "Данные для восстановления не найдены, скорее всего процесс регистрации не был до конца завершён, либо данные были повреждены."));

            var response = new RecoveryViaKeysPayloadResponse([.. recoveryKeys.Select(x => new RecoveryKeysResponse(x.EncryptedValue, x.Version))]);
            
            return Result<RecoveryViaKeysPayloadResponse>.Success(response);
        }
    }
}