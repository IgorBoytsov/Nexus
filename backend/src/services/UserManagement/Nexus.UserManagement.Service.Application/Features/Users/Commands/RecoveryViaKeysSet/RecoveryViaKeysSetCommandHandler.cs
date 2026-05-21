using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
                        
                if (user is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Не удалось найти пользователя"));

                var srp = await _context.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefaultAsync(a => a.UserId == user.Id);

                if (srp is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Способ входа по паролю не настроен."));

                srp.Update(
                    Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), 
                    CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));

                var dek = await _context.Deks.FirstOrDefaultAsync(d => d.UserId == user.Id && d.Type == DekType.Main);

                if (dek is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Ключа для шифрования не найдено."));
                    
                dek.Rotate(EncryptedValue.Create(request.EncryptedDek), CryptoVersion.Create(request.CryptoVersion));

                _context.RecoveryKeys.RemoveRange(_context.RecoveryKeys.Where(x => x.UserId == user.Id).ToList());

                List<RecoveryKey> recoveryKeys = [];
                foreach (var item in request.RecoveryKeys)
                    recoveryKeys.Add(RecoveryKey.Create(user.Id, EncryptedValue.Create(item.EncryptedValue), CryptoVersion.Create(item.CryptoVersion), KeyHint.Create("1")));
                    
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