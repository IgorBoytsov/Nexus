using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Contracts;
using Shared.Kernel.Errors;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysInit
{
    public sealed class RecoveryViaKeysInitCommandHandler(IWriteDbContext context) : IRequestHandler<RecoveryViaKeysInitCommand, Result<RecoveryViaKeysPayloadResponse>>
    {
        private readonly IWriteDbContext _context = context;

        public async Task<Result<RecoveryViaKeysPayloadResponse>> Handle(RecoveryViaKeysInitCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user is null)
                return Result<RecoveryViaKeysPayloadResponse>.Failure(new Error(ErrorCode.NotFound, "Пользователя с таким логином не существует."));

            var recoveryKeys = await _context.RecoveryKeys.Where(r => r.UserId == user.Id).ToListAsync();

            if (!recoveryKeys.Any())
                return Result<RecoveryViaKeysPayloadResponse>.Failure(new Error(AppErrors.AccountNotSetUpForRecovery, "Данные для восстановления не найдены, скорее всего процесс регистрации не был до конца завершён, либо данные были повреждены."));

            var response = new RecoveryViaKeysPayloadResponse([.. recoveryKeys.Select(x => new RecoveryKeysResponse(x.EncryptedValue, x.Version))]);
            
            return Result<RecoveryViaKeysPayloadResponse>.Success(response);
        }
    }
}