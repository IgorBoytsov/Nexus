using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed record RegisterCommand(
        string Login,
        string UserName,
        string Verifier, 
        string ClientSalt, 
        string EncryptedVerifierWrapKey,
        int CryptoVersion,
        int SrpVersion,
        string EncryptedDek, int KeyWrapVersion, string AsymmetricKeyId,
        string Email,
        Guid? IdGender, Guid? IdCountry,
        IReadOnlyCollection<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>,  
        IHasLogin,
        IHasUserName, 
        IHasVerifier, 
        IHasClientSalt, 
        IHasEncryptedDek;

    public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}