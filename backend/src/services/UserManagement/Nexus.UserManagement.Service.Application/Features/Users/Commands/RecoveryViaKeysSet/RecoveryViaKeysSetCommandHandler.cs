using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Exceptions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeysSet
{
    public sealed class RecoveryViaKeysSetCommandHandler(
        IUnitOfWork unitOfWork, 
        IUserRepository userRepository) : IRequestHandler<RecoveryViaKeysSetCommand, Result>
    {
        public async Task<Result> Handle(RecoveryViaKeysSetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<User> maybeUser = await userRepository.GetByAsync(u => u.Login == request.Login, includes: [x => x.Deks, x => x.UserAuthenticators, x => x.RecoveryKeys], clt: cancellationToken);
                        
                if (maybeUser.IsNone)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Не удалось найти пользователя"));

                User user = maybeUser.Value;

                user.UpdateSrp(Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));
                user.RotateMainDek(EncryptedValue.Create(request.EncryptedDek), CryptoVersion.Create(request.CryptoVersion));

                user.ClearRecoveryKeys();
                request.RecoveryKeys.ForEach(x => user.AddRecoveryKey(EncryptedValue.Create(x.EncryptedValue), CryptoVersion.Create(x.CryptoVersion), KeyHint.Create("1")));

                await unitOfWork.SaveChangesAsync();

                return Result.Success();
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