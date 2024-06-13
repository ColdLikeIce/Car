using CommonCore.Dependency;
using CommonCore.Enum;
using System.Reflection;

namespace HeyTripCarWeb.Share
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedDependencies(this IServiceCollection services, Type baseType, LifeCycle lifeCycle)
        {
            var interfaceType = typeof(IScopedDependency);
            var assembly = Assembly.GetExecutingAssembly(); // or any specific assembly where your services are located

            var types = assembly.GetTypes()
                .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in types)
            {
                services.AddScoped(interfaceType, type);
            }

            return services;
        }
    }
}