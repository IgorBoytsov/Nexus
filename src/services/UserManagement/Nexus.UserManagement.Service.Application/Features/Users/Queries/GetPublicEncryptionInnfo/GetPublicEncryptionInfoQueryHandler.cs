using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Shared.Contracts.UserMenagement.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed class GetPublicEncryptionInfoQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetPublicEncryptionInfoQuery, Result<PublicEncryptionInfoResponse>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<PublicEncryptionInfoResponse>> Handle(GetPublicEncryptionInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users
                    .Include(u => u.UserSecurityAssets)
                    .Include(u => u.UserAuthenticators)
                    .FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user == null)
                    return Result<PublicEncryptionInfoResponse>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var userSecirityAsset = user.UserSecurityAssets.FirstOrDefault(us => us.AssetType == AssetType.MainDek);
                var userAuthenticator = user.UserAuthenticators.FirstOrDefault(ua => ua.Method == UserAuthenticatorType.SRP);

                var userAuth = new PublicEncryptionInfoResponse(userAuthenticator!.Salt!, userSecirityAsset!.EncryptedValue);

                return Result<PublicEncryptionInfoResponse>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<PublicEncryptionInfoResponse>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}