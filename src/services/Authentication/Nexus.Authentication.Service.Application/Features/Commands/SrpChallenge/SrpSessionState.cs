namespace Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge
{
    internal record SrpSessionState(string Login, string ServerPrivateKeyB, string VerifierV, string ServerPublicKeyB);
}