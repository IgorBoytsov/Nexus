using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Shared.Contracts.Authentication.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetUserByIdQuery, Result<UserAuthDataDto>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<UserAuthDataDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users
                    .Include(u => u.UserRoles)
                    .Include(u => u.UserAuthenticators)
                    .Include(u => u.UserSecurityAssets)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

                if (user == null)
                    return Result<UserAuthDataDto>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var roleIds = user.UserRoles.Select(ur => ur.RoleId);
                var roleNames = await _writeContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync(cancellationToken);

                var userSecirityAsset = user.UserSecurityAssets.FirstOrDefault(us => us.AssetType == AssetType.MainDek);
                var userAuthenticator = user.UserAuthenticators.FirstOrDefault(ua => ua.Method == UserAuthenticatorType.SRP);

                var userAuth = new UserAuthDataDto(user.Id, user.Login, userAuthenticator!.CredentialData!, userAuthenticator!.Salt!, userSecirityAsset!.EncryptedValue, [.. roleNames.Select(rn => rn.Value)] );

                return Result<UserAuthDataDto>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<UserAuthDataDto>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}