namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed record VerifierSrpProofDTO(string M2, string TempAuthToken);
}