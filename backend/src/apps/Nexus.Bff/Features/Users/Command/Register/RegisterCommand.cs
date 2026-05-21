using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public sealed record RegisterCommand(
        string Login, string UserName, 
        string Verifier, string ClientSalt,  string EncryptedVerifierWrapKey, 
        int CryptoVersion,
        int SrpVersion,
        string Email,
        string EncryptedDek, int KekWrapVersion, string KekKeyId,
        Guid? IdGender,
        Guid? IdCountry,
        IReadOnlyCollection<RecoveryKeyCommandData> RecoveryKeys) : IRequest<Result>,    
        IHasLogin,
        IHasUserName, 
        IHasVerifier, 
        IHasClientSalt, 
        IHasEncryptedDek;

    public record RecoveryKeyCommandData(string EncryptedValue, int CryptoVersion);
}