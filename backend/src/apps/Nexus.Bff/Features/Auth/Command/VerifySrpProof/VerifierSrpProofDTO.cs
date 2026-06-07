namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed record VerifierSrpProofDTO(string SessionId, string UserId, string Login, string M2);
}