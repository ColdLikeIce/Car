using CommonCore.Dependency;
using CommonCore.Enum;
using System.Reflection;

namespace HeyTripCarWeb.Share
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedDependencies(this IServiceCollection services, Assembly assembly)
        {
            // 获取所有实现了 IScopedDependency 接口的类型
            var types = assembly.GetTypes()
                                .Where(t => t.GetInterfaces().Contains(typeof(IScopedDependency)) && t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                // 获取该类型实现的接口
                var serviceTypes = type.GetInterfaces()
                                       .Where(i => i != typeof(IScopedDependency)); // 排除标识接口

                foreach (var serviceType in serviceTypes)
                {
                    // 注册服务到 DI 容器
                    services.AddScoped(serviceType, type);
                }
            }
            return services;
        }

        public static IServiceCollection AddAutoIocImport(this IServiceCollection services, Type baseType, LifeCycle lifeCycle)
        {
            Type baseType2 = baseType;
            if (!baseType2.IsInterface)
            {
                throw new TypeLoadException("The status code must be an enumerated type");
            }

            string path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            Assembly[] source = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToArray();
            Type[] source2 = (from type in source.SelectMany((Assembly a) => a.DefinedTypes)
                              select type.AsType() into x
                              where x != baseType2 && baseType2.IsAssignableFrom(x)
                              select x).ToArray();
            Type[] array = source2.Where((Type x) => x.IsClass).ToArray();
            Type[] source3 = source2.Where((Type x) => x.IsInterface).ToArray();
            Type[] array2 = array;
            foreach (Type implementType in array2)
            {
                Type type2 = source3.FirstOrDefault((Type x) => x.IsAssignableFrom(implementType));
                if (type2 != null)
                {
                    switch (lifeCycle)
                    {
                        case LifeCycle.Singleton:
                            services.AddSingleton(type2, implementType);
                            break;

                        case LifeCycle.Transient:
                            services.AddTransient(type2, implementType);
                            break;

                        case LifeCycle.Scoped:
                            services.AddScoped(type2, implementType);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("lifeCycle", lifeCycle, null);
                    }
                }
            }

            return services;
        }
    }
}