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
                    return Result.Success();

                user.UpdateSrpVerifier(IdentityIdentifier.Create(request.Login), CredentialBlob.Create(request.Verifier), request.ClientSalt);
                user.UpdateMainDek(EncryptedAssetValue.Create(request.EncryptedDek), EncryptionMetadata.Create(request.EncryptionAlgorithm, request.Iterations, request.KdfType));

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
