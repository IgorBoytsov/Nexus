using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed class GetPublicEncryptionInfoQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetPublicEncryptionInfoQuery, Result<PublicEncryptionInfoDTO>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<PublicEncryptionInfoDTO>> Handle(GetPublicEncryptionInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user == null)
                    return Result<PublicEncryptionInfoDTO>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var userAuth = new PublicEncryptionInfoDTO(user.ClientSalt, user.EncryptedDek);

                return Result<PublicEncryptionInfoDTO>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<PublicEncryptionInfoDTO>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}