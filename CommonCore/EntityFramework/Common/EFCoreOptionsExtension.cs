using CommonCore.EntityFramework.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    public class EFCoreOptionsExtension<TDbContext>
     where TDbContext : DbContext
    {
        private readonly Action<EFCoreOptions<TDbContext>> _configure;

        public EFCoreOptionsExtension(Action<EFCoreOptions<TDbContext>> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            var options = new EFCoreOptions<TDbContext>();
            _configure(options);
            services.AddDbContext<TDbContext>();
            services.Configure(_configure);
            services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager<TDbContext>>();
            services.AddScoped<IUnitOfWorkCompleteHandle, UnitOfWorkCompleteHandle<TDbContext>>();
            services.AddScoped<IDbContextProvider<TDbContext>, DbContextProvider<TDbContext>>();
        }
    }
}