using System.Text.Json;
using Shared.Web.Extensions;

namespace Nexus.Bff.Extensions
{
    public static class ConfigureOptionsServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureOptions(this IServiceCollection services)
        {
            services.Configure<JsonSerializerOptions>(opt => opt.AddCrossdyneDefaults());

            return services;
        }
    }
}