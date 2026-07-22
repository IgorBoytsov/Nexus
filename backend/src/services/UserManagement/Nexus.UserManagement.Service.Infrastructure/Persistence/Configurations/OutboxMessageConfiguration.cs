using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages", "outbox");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(x => x.EventType)
                .HasColumnName("event_type")
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnName("content")
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.OccurredOnUtc)
                .HasColumnName("occurred_on_utc")
                .IsRequired();

            builder.Property(x => x.ProcessedOnUtc)
                .HasColumnName("processed_on_utc");

            builder.Property(x => x.RetryCount)
                .HasColumnName("retry_count")
                .IsRequired();

            builder.Property(x => x.Error)
                .HasColumnName("error")
                .HasColumnType("text");

            builder.Property(x => x.NextRetryAt)
                .HasColumnName("next_retry_at")
                .IsRequired();

            builder.HasIndex(x => x.NextRetryAt)
                .HasDatabaseName("IX_outbox_messages_pending")
                .HasFilter("processed_on_utc IS NULL")
                .IncludeProperties(x => new 
                { 
                    x.OccurredOnUtc,
                    x.EventType, 
                    x.Content, 
                    x.RetryCount, 
                    x.Error 
                });

            builder.HasIndex(x => new { x.EventType, x.OccurredOnUtc })
                .HasDatabaseName("IX_outbox_messages_event_type_occurred");

            builder.HasIndex(x => x.RetryCount)
                .HasDatabaseName("IX_outbox_messages_retry_count")
                .HasFilter("retry_count > 0"); 
        }
    }
}