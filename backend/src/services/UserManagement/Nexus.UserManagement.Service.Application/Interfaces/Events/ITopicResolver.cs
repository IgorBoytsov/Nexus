namespace Nexus.UserManagement.Service.Application.Interfaces.Events
{
    public interface ITopicResolver
    {
        string Resolve(string eventType);
    }
}