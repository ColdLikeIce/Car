using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        TDbContext GetDbContext();
    }
}