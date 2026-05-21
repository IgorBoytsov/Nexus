using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(IWriteDbContext writeContext) : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

                if (user is null)
                    return Result.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная ошибка на стороне сервера."));

                var srp = await _writeContext.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefaultAsync(a => a.UserId == user.Id);

                if (srp is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Способ входа по паролю не настроен."));

                srp.Update(
                    Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), 
                    CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));

                var dek = await _writeContext.Deks.FirstOrDefaultAsync(d => d.UserId == user.Id && d.Type == DekType.Main);

                if (dek is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Ключа для шифрования не найдено."));

                dek.Rotate(EncryptedValue.Create(request.EncryptedDek), CryptoVersion.Create(request.CryptoVersion));
            
                _writeContext.RecoveryKeys.RemoveRange(_writeContext.RecoveryKeys.Where(x => x.UserId == user.Id).ToList());

                List<RecoveryKey> recoveryKeys = [];
                foreach (var item in request.RecoveryKeys)
                    recoveryKeys.Add(RecoveryKey.Create(user.Id, EncryptedValue.Create(item.EncryptedValue), CryptoVersion.Create(item.CryptoVersion), KeyHint.Create("1")));

                await _writeContext.RecoveryKeys.AddRangeAsync(recoveryKeys, cancellationToken);

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
