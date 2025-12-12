using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Contracts.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetByLoginInternal
{
    public sealed class GetUserByLoginInternalQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetUserByLoginInternalQuery, Result<UserAuthDataDto>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<UserAuthDataDto>> Handle(GetUserByLoginInternalQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users.Include(x => x.UserRoles).FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user == null)
                    return Result<UserAuthDataDto>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var roleIds = user.UserRoles.Select(ur => ur.RoleId);
                var roleNames = await _writeContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync(cancellationToken);

                var userAuth = new UserAuthDataDto(user.Id, user.Login, user.PasswordHash, user.ClientSalt, user.EncryptedDek, [.. roleNames.Select(rn => rn.Value)]);

                return Result<UserAuthDataDto>.Success(userAuth);
            } 
            catch (Exception)
            {
                return Result<UserAuthDataDto>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}