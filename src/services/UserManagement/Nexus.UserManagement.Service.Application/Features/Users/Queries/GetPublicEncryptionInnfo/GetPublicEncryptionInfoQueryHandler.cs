using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
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
                var user = await _writeContext.Users.Include(u => u.Credentials).FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user == null)
                    return Result<PublicEncryptionInfoResponse>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var userAuth = new PublicEncryptionInfoResponse(user.Credentials.ClientSalt, user.Credentials.EncryptedDek);

                return Result<PublicEncryptionInfoResponse>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<PublicEncryptionInfoResponse>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}