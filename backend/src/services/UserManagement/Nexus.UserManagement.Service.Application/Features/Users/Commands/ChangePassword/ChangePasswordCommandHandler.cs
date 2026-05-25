using Crossdyne.Toolkit.Primitives;
using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork, 
        IUserRepository userRepository) : IRequestHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Maybe<User> maybeUser = await userRepository.GetByAsync(u => u.Id == request.UserId, includes: [u => u.Deks, u => u.UserAuthenticators], clt: cancellationToken);

                if (maybeUser.IsNone)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден"));

                User user = maybeUser.Value;

                user.UpdateSrp(Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));
                user.RotateMainDek(EncryptedValue.Create(request.EncryptedDek), CryptoVersion.Create(request.CryptoVersion));

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Server, "Произошла критическая ошибки на стороне сервера при восстановление доступа"));
            }

        }
    }
}