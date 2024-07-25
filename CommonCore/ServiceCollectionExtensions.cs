using CommonCore.EntityFramework.Common;
using CommonCore.EntityFramework.UnitOfWork;
using CommonCore.Enum;
using CommonCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMapper(this IServiceCollection services)
       => services.AddSingleton<IMapper, MapperManager>();

        /// <summary>
        /// Add EntityFramework
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <param name="isUseLogger"></param>
        /// <returns></returns>
        public static IServiceCollection AddHyTripEntityFramework<TDbContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TDbContext : DbContext
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            services.AddDbContext<TDbContext>(options, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager<TDbContext>>();
            services.AddScoped<IDbContextProvider<TDbContext>, DbContextProvider<TDbContext>>();
            services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            return services;
        }

        /// <summary>
        /// Add auto ioc services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="baseType"></param>
        /// <param name="lifeCycle"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoIoc(this IServiceCollection services, Type baseType, LifeCycle lifeCycle)
        {
            if (!baseType.IsInterface)
            {
                throw new TypeLoadException("The status code must be an enumerated type");
            }

            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = System.IO.Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToArray();
            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).ToArray();
            var interfaceTypes = types.Where(x => x.IsInterface).ToArray();
            foreach (var implementType in implementTypes)
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                if (interfaceType != null)
                    switch (lifeCycle)
                    {
                        case LifeCycle.Singleton:
                            services.AddSingleton(interfaceType, implementType);
                            break;

                        case LifeCycle.Transient:
                            services.AddTransient(interfaceType, implementType);
                            break;

                        case LifeCycle.Scoped:
                            services.AddScoped(interfaceType, implementType);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifeCycle), lifeCycle, null);
                    }
            }
            return services;
        }
    }
}