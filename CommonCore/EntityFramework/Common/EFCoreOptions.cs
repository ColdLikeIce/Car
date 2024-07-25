using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    /// <summary>
    /// Entity Framework Core Options
    /// </summary>
    public class EFCoreOptions<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the database's connection string that will be used to store database entities.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DbConnection ExistingConnection { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }

        internal string Version { get; set; }

        public EFCoreOptions()
        {
            DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
            Version = "1.0.0";
        }
    }
}