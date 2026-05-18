using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Auth.Command.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Login,
        string Verifier, 
        string ClientSalt, 
        string EncryptedDek,
        int CryptoVersion, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId) : IRequest<Result>,
        IHasLogin, 
        IHasVerifier, 
        IHasClientSalt, 
        IHasEncryptedDek;
}