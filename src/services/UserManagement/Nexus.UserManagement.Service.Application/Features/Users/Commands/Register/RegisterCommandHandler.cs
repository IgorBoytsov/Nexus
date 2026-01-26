using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.SmartEnums;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed class RegisterCommandHandler(IWriteDbContext writeContext) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = User.Create(request.Login, request.UserName, request.Email, request.Phone, EnumStatus.Active.Id, request.IdGender, request.IdCountry);

                user.AddRole(RoleId.From(EnumRole.User.Id));
                user.AddUserAuthenticator(UserAuthenticatorType.SRP, IdentityIdentifier.Create(request.Login), CredentialBlob.Create(request.Verifier), request.ClientSalt);
                user.AddUserAuthenticator(UserAuthenticatorType.Email, IdentityIdentifier.Create(request.Email), credentialBlob: null, salt: null);
                user.AddUserSecurityAssets(AssetType.MainDek, EncryptedAssetValue.Create(request.EncryptedDek), EncryptionMetadata.Create(request.EncryptionAlgorithm, request.Iterations, request.KdfType));

                if (!string.IsNullOrWhiteSpace(request.Phone))
                    user.AddUserAuthenticator(UserAuthenticatorType.Phone, IdentityIdentifier.Create(request.Phone), credentialBlob: null, salt: null);

                await _writeContext.Users.AddAsync(user, cancellationToken);
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