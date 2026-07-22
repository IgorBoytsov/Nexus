using System.Threading.Channels;

namespace Nexus.UserManagement.Service.Application.Interfaces.Outbox
{
    public interface IOutboxSignal
    {
        void Signal();
        ChannelReader<bool> Reader { get; }
    }
}