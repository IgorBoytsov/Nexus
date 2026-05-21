using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.v1;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed class GetPublicEncryptionInfoQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetPublicEncryptionInfoQuery, Result<PublicEncryptionInfoResponse>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<PublicEncryptionInfoResponse>> Handle(GetPublicEncryptionInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user == null)
                    return Result<PublicEncryptionInfoResponse>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var dek = await _writeContext.Deks.FirstOrDefaultAsync(d => d.Type == DekType.Main);
                var srp = await _writeContext.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefaultAsync(ua => ua.Method == UserAuthenticatorType.SRP);

                var userAuth = new PublicEncryptionInfoResponse(srp!.Salt!, dek!.EncryptedValue);

                return Result<PublicEncryptionInfoResponse>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<PublicEncryptionInfoResponse>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}