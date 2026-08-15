namespace Nexus.UserManagement.Service.Domain.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime OccurredOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }

        // Retry / Dead Letter поля
        public int RetryCount { get; set; }
        public string? Error { get; set; }
        public DateTime NextRetryAt { get; set; }
    }
}