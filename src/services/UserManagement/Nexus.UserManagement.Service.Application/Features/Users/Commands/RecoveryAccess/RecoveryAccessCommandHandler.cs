using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Results;
using Shared.Security.Hasher;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryAccess
{
    public sealed class RecoveryAccessCommandHandler(IWriteDbContext writeContext, IPasswordHasher hasher) : IRequestHandler<RecoveryAccessCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;
        private readonly IPasswordHasher _hasher = hasher;

        public async Task<Result> Handle(RecoveryAccessCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login && u.Email == request.Email, cancellationToken);

                if (user is null)
                    return Result.Success();

                var newPasswordHash = PasswordHash.Create(_hasher.HashPassword(request.NewPassword));

                user.UpdateVerifier(newPasswordHash);

                await _writeContext.SaveChangesAsync(cancellationToken);

                user.ClearDomainEvents();

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Server, "Произошла критическая ошибки на стороне сервера при восстановление доступа"));
            }
        }
    }
}
