using AutoMapper;
using Nexus.UserManagement.Service.Domain.Events;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.UserManagement.Service.Application.Mapping.Events
{
    public sealed class PasswordResetCodeGeneratedEventMapper : Profile
    {
        public PasswordResetCodeGeneratedEventMapper()
        {
            CreateMap<PasswordResetRequestedDomainEvent, PasswordResetRequestedIntegrationEvent>()
                .ConstructUsing(src => new PasswordResetRequestedIntegrationEvent(
                    IdEvent: src.IdEvent,
                    OccurredOnUtc: src.OccurredOnUtc.ToString("O"),
                    UserId: src.UserId.Value,
                    To: src.Email,
                    Subject: "Код для подтверждения сброса пароля",
                    Body: $"Ваш код подтверждения: {src.Code}\n\nКод действителен 10 минут.",
                    ExpiresAt: src.ExpiresAt.ToString("O")
                ));
        }
    }
}