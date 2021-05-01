using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Integration
{
    public static class DependencyInjection
    {
        public static IServiceCollection ReplaceServicesWithTestServices(this IServiceCollection services)
        {

            return services;
        }

        public static IServiceCollection RemoveService<T>(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(T));

            if (descriptor != null) services.Remove(descriptor);

            return services;
        }
    }
}