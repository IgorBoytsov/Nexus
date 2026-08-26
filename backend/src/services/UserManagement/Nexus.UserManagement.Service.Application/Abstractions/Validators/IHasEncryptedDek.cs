namespace Nexus.UserManagement.Service.Application.Abstractions.Validators
{
    public interface IHasEncryptedDek
    {
        public string EncryptedVerifierWrapKey { get; }
    }
}