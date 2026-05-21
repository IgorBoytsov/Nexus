using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Exceptions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysSet
{
    public sealed class RecoveryViaKeysSetCommandHandler(IWriteDbContext context) : IRequestHandler<RecoveryViaKeysSetCommand, Result>
    {
        private readonly IWriteDbContext _context = context;

        public async Task<Result> Handle(RecoveryViaKeysSetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserAuthenticators)
                    .Include(u => u.UserSecurityAssets)
                        .FirstOrDefaultAsync(u => u.Login == request.Login);

                if (user is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Не удалось найти пользователя"));

                user.UpdateSrpAuthenticator(
                    Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), 
                    CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));
                user.UpdateMainDek(EncryptedAssetValue.Create(request.EncryptedDek), request.CryptoVersion);

                user.ClearRecoveryKeys();
                request.RecoveryKeys.ForEach(rk => user.AddUserSecurityAssets(AssetType.RecoveryKey, EncryptedAssetValue.Create(rk.EncryptedValue), rk.CryptoVersion));

                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (DbUpdateException)
            {
                return Result.Failure(new Error(ErrorCode.Save, "Произошла критическая ошибка на стороне сервера при обновление данных."));
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Error);
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.Server, $"Произошла непредвиденная ошибка на стороне сервера {ex}"));
            }
        }
    }
}