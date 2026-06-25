using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Validation.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed record RegisterCommand(
        // Общая информация об аккаунте
        string Login, 
        string UserName, 
        string Email, 
        Guid? IdGender, 
        Guid? IdCountry,
        // Srp
        string EncryptedVerifier, 
        string SrpSalt, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId,
        // Dek 
        string EncryptedDek, 
        string DekSalt, 
        int CryptoVersion,
        IReadOnlyCollection<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>,  
        IHasLogin,
        IHasUserName, 
        IHasEncryptedVerifier, 
        IHasSrpSalt, 
        IHasEncryptedDek;

    public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}