using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.SmartEnums;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Crossdyne.Toolkit.Results;
using Shared.Kernel.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed class RegisterCommandHandler(IWriteDbContext writeContext) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = User.Create(request.Login, request.UserName, request.Email, EnumStatus.Active.Id, request.IdGender, request.IdCountry);
                var dek = Dek.Create(user.Id, EncryptedValue.Create(request.EncryptedDek), CryptoVersion.Create(request.CryptoVersion), DekType.Main);
                var srp = SrpAuthenticator.Create(user.Id, Login.Create(request.Login), Verificator.Create(request.Verifier), Salt.Create(request.ClientSalt), SrpVersion.Create(request.SrpVersion), CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId));
                var email = EmailAuthenticator.Create(user.Id, Email.Create(request.Email));

                List<RecoveryKey> recoveryKeys = [];
                foreach (var item in request.RecoveryKeys)
                    recoveryKeys.Add(RecoveryKey.Create(user.Id, EncryptedValue.Create(item.EncryptedValue), CryptoVersion.Create(item.CryptoVersion), KeyHint.Create("1")));
                
                user.AddRole(RoleId.From(EnumRole.User.Id));

                await _writeContext.Users.AddAsync(user, cancellationToken);
                await _writeContext.Deks.AddAsync(dek, cancellationToken);
                await _writeContext.RecoveryKeys.AddRangeAsync(recoveryKeys, cancellationToken);
                await _writeContext.UserAuthenticators.AddRangeAsync(srp, email);

                await _writeContext.SaveChangesAsync(cancellationToken);

                user.ClearDomainEvents();

                return Result.Success();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Result.Failure(new Error(ErrorCode.Conflict, "Email уже занят."));
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Error);
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Save, "Произошла критическая ошибка на стороне сервера при регистрации"));
            }
        }
    }
}