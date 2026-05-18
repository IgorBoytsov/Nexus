using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Crossdyne.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(IWriteDbContext writeContext) : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users
                    .Include(x => x.UserAuthenticators)
                    .Include(x => x.UserSecurityAssets)
                        .FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user is null)
                    return Result.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная ошибка на стороне сервера."));

                user.UpdateSrpAuthenticator(
                    Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), 
                    CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));
                user.UpdateMainDek(EncryptedAssetValue.Create(request.EncryptedVerifierWrapKey), request.CryptoVersion);

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
