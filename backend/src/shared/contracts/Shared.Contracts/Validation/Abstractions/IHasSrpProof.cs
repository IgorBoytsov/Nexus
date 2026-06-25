namespace Shared.Contracts.Validation.Abstractions
{
    public interface IHasSrpProof
    {
        string A { get; }
        string M1 { get; }
    }
}