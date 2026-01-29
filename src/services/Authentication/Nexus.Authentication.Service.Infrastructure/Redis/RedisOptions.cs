namespace Nexus.Authentication.Service.Infrastructure.Redis
{
    public class RedisOptions
    {
        public const string SectionName = "Redis";
        public string ConnectionString { get; set; }
        public int Database { get; set; } = 0;
    }
}
