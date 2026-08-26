namespace Shared.Abstractions.Messaging.Abstractions
{
    public interface IIntegrationEvent 
    {
        Guid IdEvent { get; }
        DateTime OccurredOnUtc { get; }
    }
}