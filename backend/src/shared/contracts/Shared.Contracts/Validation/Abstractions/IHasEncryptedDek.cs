namespace Shared.Contracts.Validation.Abstractions
{
    public interface IHasEncryptedDek
    {
        public string EncryptedVerifierWrapKey { get; }
    }
}