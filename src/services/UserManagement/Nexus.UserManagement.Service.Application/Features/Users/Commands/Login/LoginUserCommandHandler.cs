using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById;
using Shared.Kernel.Results;
using Shared.Security.Hasher;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Login
{
    public sealed class LoginUserCommandHandler(
        IWriteDbContext writeContext,
        IPasswordHasher hasher) : IRequestHandler<LoginUserCommand, Result<UserDto>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;
        private readonly IPasswordHasher _hasher = hasher;

        public async Task<Result<UserDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var storageUser = await _writeContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (storageUser is null)
                    return Result<UserDto>.Failure(new Error(ErrorCode.NotFound, "Неверный логин или пароль."));

                var verifiablePassword = _hasher.VerifyPassword(request.Password, storageUser.PasswordHash);

                if (!verifiablePassword)
                    return Result<UserDto>.Failure(new Error(ErrorCode.InvalidPassword, "Неверный логин или пароль."));

                storageUser.UpdateLastEntryDate();

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result<UserDto>.Success(new UserDto(storageUser.Id, storageUser.Login, storageUser.UserName, storageUser.Email, storageUser?.Phone?.ToString(), storageUser!.IdStatus.ToString(), storageUser.DateRegistration, storageUser.DateEntry));
            }
            catch (Exception)
            {
                return Result<UserDto>.Failure(new Error(ErrorCode.Server, "Произошла критическая ошибки на стороне сервера при аутентификации"));
            }
        }
    }
}