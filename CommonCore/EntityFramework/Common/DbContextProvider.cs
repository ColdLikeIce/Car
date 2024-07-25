using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
          where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public DbContextProvider(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TDbContext GetDbContext()
        {
            return _dbContext;
        }
    }
}