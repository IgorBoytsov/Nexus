using AutoMapper;
using Nexus.UserManagement.Service.Domain.Events;
using Shared.Contracts.UserManagement.Events;

namespace Nexus.UserManagement.Service.Application.Mapping.Events
{
    public sealed class ChangeEmailRequestedEventMapper : Profile
    {
        public ChangeEmailRequestedEventMapper()
        {
            CreateMap<ChangeEmailRequestedDomainEvent, ChangeEmailRequestedIntegrationEvent>()
                .ConstructUsing(src => new ChangeEmailRequestedIntegrationEvent(
                    IdEvent: src.IdEvent,
                    OccurredOnUtc: src.OccurredOnUtc,
                    UserId: src.UserId.Value.ToString(),
                    To: src.Email.Value,
                    Subject: "Код для подтверждения смены адреса электриной почты",
                    Body: $"Ваш код подтверждения: {src.Code}\n\nКод действителен 10 минут",
                    ExpiresAt: src.ExpiresAt.ToString("O")
                ));
        }
    }
}