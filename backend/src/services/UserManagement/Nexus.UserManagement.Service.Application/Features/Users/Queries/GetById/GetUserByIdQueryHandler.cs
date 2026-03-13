using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.V1;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetUserByIdQuery, Result<UserAuthDataResponse>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<UserAuthDataResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users
                    .Include(u => u.UserRoles)
                    .Include(u => u.UserAuthenticators)
                    .Include(u => u.UserSecurityAssets)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

                if (user == null)
                    return Result<UserAuthDataResponse>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var roleIds = user.UserRoles.Select(ur => ur.RoleId);
                var roleNames = await _writeContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync(cancellationToken);

                var userSecurityAsset = user.UserSecurityAssets.FirstOrDefault(us => us.AssetType == AssetType.MainDek);
                var userAuthenticator = user.UserAuthenticators.FirstOrDefault(ua => ua.Method == UserAuthenticatorType.SRP);

                var userAuth = new UserAuthDataResponse
                {
                    Id = user.Id.Value.ToString(),
                    Login = user.Login,
                    Verifier = userAuthenticator!.CredentialData,
                    ClientSalt = userAuthenticator.Salt,
                    EncryptedDek = userSecurityAsset!.EncryptedValue,
                };

                userAuth.Roles.AddRange(roleNames.Select(rn => rn.Value));

                return Result<UserAuthDataResponse>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<UserAuthDataResponse>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}