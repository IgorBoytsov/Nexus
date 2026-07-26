namespace Shared.Contracts.Messaging.Interfaces
{
    public interface IIntegrationEvent 
    {
        Guid IdEvent { get; }
        DateTime OccurredOnUtc { get; }
    }
}