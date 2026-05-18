namespace Shared.Validations.Common.Abstractions
{
    public interface IHasEncryptedDek
    {
        public string EncryptedVerifierWrapKey { get; }
    }
}