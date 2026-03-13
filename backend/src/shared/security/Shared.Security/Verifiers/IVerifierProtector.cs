namespace Shared.Security.Verifiers
{
    public interface IVerifierProtector
    {
        string Protect(string verifier);
        string Unprotect(string protectedVerifier);
    }
}