using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    public interface IBaseRepository<TDbContext>
     where TDbContext : DbContext
    {
        IRepositoryBase<TDbContext, TEntity> GetRepository<TEntity>()
            where TEntity : class;

        /// <summary>
        /// 执行存储过程的扩展方法ExecuteSqlCommand
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlCommandNonQueryAsync(CommandType commandType, string sql, [NotNull] params object[] parameters);

        /// <summary>
        /// 执行存储过程的扩展方法ExecuteSqlCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandType"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<List<T>> ExecuteSqlCommandAsync<T>(CommandType commandType, string sql, [NotNull] params object[] parameters)
            where T : class, new();

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        ///
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);

        Task<List<T>> ExecuteSqlToListAsync<T>(string sql, [MaybeNull] params object[] parameters)
            where T : class, new();

        Task<T> ExecuteSqlToEntityAsync<T>(string sql, [MaybeNull] params object[] parameters)
            where T : class, new();

        Task<int> ExecuteSqlToCountAsync(string sql, [MaybeNull] params object[] parameters);

        Task<long> ExecuteSqlToCountLongAsync(string sql, [MaybeNull] params object[] parameters);
    }
}