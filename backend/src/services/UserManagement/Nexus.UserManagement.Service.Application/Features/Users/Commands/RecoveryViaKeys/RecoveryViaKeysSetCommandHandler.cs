using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeys
{
    public sealed class RecoveryViaKeysCommandHandler(
        IUnitOfWork unitOfWork, 
        IUserRepository userRepository) : IRequestHandler<RecoveryViaKeysCommand, Result>
    {
        public async Task<Result> Handle(RecoveryViaKeysCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybeUser = await userRepository.GetByAsync(u => u.Login == request.Login, includes: [x => x.Deks, x => x.UserAuthenticators, x => x.RecoveryKeys], clt: cancellationToken);
                    
            if (maybeUser.IsNone)
                return Result.Failure(new Error(ErrorCode.NotFound, "Не удалось найти пользователя"));

            User user = maybeUser.Value;

            user.ChangePassword(
                Verificator.Create(request.EncryptedVerifier), Salt.Create(request.SrpSalt), SrpVersion.Create(request.SrpVersion), CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId),
                EncryptedValue.Create(request.EncryptedDek), Salt.Create(request.DekSalt), CryptoVersion.Create(request.CryptoVersion));

            user.ClearRecoveryKeys();
            request.RecoveryKeys.ForEach(x => user.AddRecoveryKey(EncryptedValue.Create(x.EncryptedValue), CryptoVersion.Create(x.CryptoVersion), KeyHint.Create("1")));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}