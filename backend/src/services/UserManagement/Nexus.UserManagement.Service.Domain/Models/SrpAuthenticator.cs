using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class SrpAuthenticator : UserAuthenticator
    {
        public Login? Login { get; private set; }
        public Verificator? EncryptedVerifier { get; private set; }
        public Salt? Salt { get; private set; }
        public SrpVersion? SrpVersion { get; private set; }
        public CredentialBlob? EncryptedVerifierWrapKey { get; private set; }
        public CryptoVersion? KeyWrapVersion { get; private set; }
        public AsymmetricKeyId? AsymmetricKeyId { get; private set; }
        public CryptoVersion? CryptoVersion { get; private set; }

        private SrpAuthenticator()
        {
            
        }

        private SrpAuthenticator(
            UserId userId,
            Login login, Verificator encryptedVerifier, Salt salt, SrpVersion srpVersion,
            CredentialBlob encryptedVerifierWrapKey, CryptoVersion keyWrapVersion, AsymmetricKeyId asymmetricKeyId, 
            CryptoVersion cryptoVersion) : base(UserAuthenticatorId.New(), userId, UserAuthenticatorType.SRP)
        {
            Login = login;
            EncryptedVerifier = encryptedVerifier;
            Salt = salt;
            SrpVersion = srpVersion;
            EncryptedVerifierWrapKey = encryptedVerifierWrapKey;
            KeyWrapVersion = keyWrapVersion;
            AsymmetricKeyId = asymmetricKeyId;
            CryptoVersion = cryptoVersion;
        }

        internal static SrpAuthenticator Create(
            UserId userId, 
            Login login, Verificator verificator, Salt salt, SrpVersion srpVersion,
            CredentialBlob encryptedVerifierWrapKey, CryptoVersion keyWrapVersion, AsymmetricKeyId asymmetricKeyId,
            CryptoVersion cryptoVersion)
        {
            return new SrpAuthenticator(userId, login, verificator, salt, srpVersion, encryptedVerifierWrapKey, keyWrapVersion, asymmetricKeyId, cryptoVersion);
        }

        internal void Update(Verificator verifier, Salt salt, SrpVersion srpVersion, CredentialBlob encryptedVerifierWrapKey, CryptoVersion keyWrapVersion, AsymmetricKeyId asymmetricKeyId, CryptoVersion cryptoVersion)
        {
            EncryptedVerifier = verifier;
            Salt = salt;
            SrpVersion = srpVersion;
            EncryptedVerifierWrapKey = encryptedVerifierWrapKey;
            KeyWrapVersion = keyWrapVersion;
            AsymmetricKeyId = asymmetricKeyId;
            CryptoVersion = cryptoVersion;
        }

        internal Verificator GetVerificator() => EncryptedVerifier ?? throw new InvalidOperationException("SRP Verificator not initialized");
    }
}