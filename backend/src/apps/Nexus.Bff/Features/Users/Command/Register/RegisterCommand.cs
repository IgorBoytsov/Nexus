using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Users.Command.Register
{
    public sealed record RegisterCommand(
        string Login, string UserName, 
        string Verifier, string ClientSalt,  string EncryptedDek, 
        int CryptoVersion,
        string Email,
        Guid? IdGender,
        Guid? IdCountry) : IRequest<Result>,    
        IHasLogin,
        IHasUserName, 
        IHasVerifier, 
        IHasClientSalt, 
        IHasEncryptedDek;
}