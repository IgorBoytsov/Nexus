namespace Nexus.Authentication.Service.Application.Abstractions.Validators
{
    public interface IHasSrpProof
    {
        string A { get; }
        string M1 { get; }
    }
}