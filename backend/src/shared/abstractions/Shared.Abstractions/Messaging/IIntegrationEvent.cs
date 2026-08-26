namespace Shared.Abstractions.Messaging
{
    public interface IIntegrationEvent 
    {
        Guid IdEvent { get; }
        DateTime OccurredOnUtc { get; }
    }
}