using System.Threading.Channels;
using Nexus.UserManagement.Service.Application.Interfaces.Outbox;

namespace Nexus.UserManagement.Service.Infrastructure.Outbox
{
    public sealed class OutboxSignal : IOutboxSignal, IDisposable
    {
        private readonly Channel<bool> _channel = Channel.CreateBounded<bool>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });

        public ChannelReader<bool> Reader => _channel.Reader;

        public void Signal() => _channel.Writer.TryWrite(true);

        public void Dispose() => _channel.Writer.Complete();
    }
}