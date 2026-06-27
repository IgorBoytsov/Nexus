using MediatR;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.SmartEnums;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Crossdyne.Toolkit.Results;
using Shared.Kernel.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed class RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository) : IRequestHandler<RegisterCommand, Result>
    {
        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await userRepository.CheckAvailableEmail(Email.Create(request.Email)))
                return Result.Failure(new Error(ErrorCode.Conflict, "Email уже занят."));

            var user = User.Create(request.Login, request.UserName, request.Email, EnumStatus.Active.Id, request.IdGender, request.IdCountry);
            user.AddMainDek(EncryptedValue.Create(request.EncryptedDek), Salt.Create(request.DekSalt), CryptoVersion.Create(request.CryptoVersion));
            user.AddSrpAuthenticator(Login.Create(request.Login), Verificator.Create(request.EncryptedVerifier), Salt.Create(request.SrpSalt), SrpVersion.Create(request.SrpVersion), CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));
            user.AddEmailAuthenticator(Email.Create(request.Email));
            request.RecoveryKeys.ToList().ForEach(x => user.AddRecoveryKey(EncryptedValue.Create(x.EncryptedValue), CryptoVersion.Create(x.CryptoVersion), KeyHint.Create("1")));
            user.AddRole(RoleId.From(EnumRole.User.Id));

            await userRepository.AddAsync(user, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            user.ClearDomainEvents();

            return Result.Success();
        }
    }
}