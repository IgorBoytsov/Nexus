using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangePasswordInit
{
    public sealed class ChangePasswordInitCommandHandler(IUserRepository userRepository) : IRequestHandler<ChangePasswordInitCommand, Result<ChangePasswordInitResponse>>
    {
        public async Task<Result<ChangePasswordInitResponse>> Handle(ChangePasswordInitCommand request, CancellationToken cancellationToken)
        {
            var maybeUser = await userRepository.GetByAsync(x => x.Id == request.UserId, includes: [x => x.Deks, x => x.UserAuthenticators], clt: cancellationToken);

            var user = maybeUser.Value;

            var dek = user.Deks.FirstOrDefault(x => x.Type == DekType.Main);
            var srp = user.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault(x => x.Method == UserAuthenticatorType.SRP);

            var response = new ChangePasswordInitResponse(
                user.Login,
                dek!.EncryptedValue,
                dek.Salt, 
                dek.Version, 
                srp!.AsymmetricKeyId!.Value);

            return Result<ChangePasswordInitResponse>.Success(response);
        }
    }
}