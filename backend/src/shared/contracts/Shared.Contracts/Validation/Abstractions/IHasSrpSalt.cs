namespace Shared.Contracts.Validation.Abstractions
{
    public interface IHasSrpSalt
    {
        public string SrpSalt { get; }
    }
}