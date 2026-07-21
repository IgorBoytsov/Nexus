using MediatR;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Crossdyne.Toolkit.Primitives;
using Shared.Contracts.Messaging.Interfaces;
using Nexus.UserManagement.Service.Domain.Events;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IEventPublisher eventPublisher) : IRequestHandler<ResetPasswordCommand, Result>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybeUser = await userRepository.GetByAsync(x => x.Login == request.Login, includes: [x => x.UserAuthenticators, x => x.Deks, x => x.RecoveryKeys] ,clt: cancellationToken);

            if (maybeUser.IsNone)
                return Result.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная ошибка на стороне сервера."));

            User user = maybeUser.Value;
          
            user.ResetPassword(
                Verificator.Create(request.EncryptedVerifier), Salt.Create(request.SrpSalt), SrpVersion.Create(request.SrpVersion), CredentialBlob.Create(request.EncryptedVerifierWrapKey), CryptoVersion.Create(request.KeyWrapVersion), AsymmetricKeyId.Create(request.AsymmetricKeyId),
                EncryptedValue.Create(request.EncryptedDek), Salt.Create(request.DekSalt), CryptoVersion.Create(request.CryptoVersion));

            request.RecoveryKeys.ForEach(x => user.AddRecoveryKey(EncryptedValue.Create(x.EncryptedValue), CryptoVersion.Create(x.CryptoVersion), KeyHint.Create("1")));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var events = user.GetDomainEvents();

            foreach (var domainEvent in events)
            {
                if (domainEvent is UserPasswordResetDomainEvent resetEvent)
                {
                    var integrationEvent = new UserPasswordResetIntegrationEvent(resetEvent.IdEvent.ToString(), resetEvent.OccurredOnUtc.ToString(), resetEvent.UserId.Value.ToString());
                    await eventPublisher.PublishAsync("user-management.user.password-reset", integrationEvent);
                }
            }

            user.ClearDomainEvents();

            return Result.Success();
        }
    }
}
