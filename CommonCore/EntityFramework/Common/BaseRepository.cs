using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CommonCore.EntityFramework.Common
{
    public class BaseRepository<TDbContext> : IBaseRepository<TDbContext>
         where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TDbContext _context;

        public BaseRepository(
            IServiceProvider serviceProvider,
            TDbContext dbContext
            )
        {
            _serviceProvider = serviceProvider;
            _context = dbContext;
        }

        public IRepositoryBase<TDbContext, TEntity> GetRepository<TEntity>()
            where TEntity : class
            => _serviceProvider.GetRequiredService<IRepositoryBase<TDbContext, TEntity>>();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
            => await _context.Database.BeginTransactionAsync();

        /// <summary>
        /// 创建SqlCommand
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="transaction"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<DbCommand> CreateCommand(string sql, params object[] parameters)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);
            return cmd;
        }

        /// <summary>
        /// 执行存储过程的扩展方法ExecuteSqlCommand
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlCommandNonQueryAsync(CommandType commandType, string sql, [NotNull] params object[] parameters)
        {
            //创建SqlCommand
            var command = await CreateCommand(sql, parameters);
            var conn = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            _context.Database.UseTransaction(transaction);
            command.CommandType = commandType;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<List<T>> ExecuteSqlCommandAsync<T>(CommandType commandType, string sql, [NotNull] params object[] parameters)
            where T : class, new()
        {
            //创建SqlCommand
            var command = await CreateCommand(sql, parameters);
            var conn = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            _context.Database.UseTransaction(transaction);
            command.CommandType = commandType;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            var reader = await command.ExecuteReaderAsync();
            List<T> list = new();
            Type type = typeof(T);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var note = Activator.CreateInstance(type);
                    var columns = reader.GetColumnSchema();
                    foreach (var item in type.GetProperties())
                    {
                        if (!columns.Any(x => x.ColumnName.ToLower() == item.Name.ToLower())) continue;
                        var value = reader[item.Name];
                        if (!item.CanWrite || value is DBNull || value == DBNull.Value) continue;
                        try
                        {
                            #region SetValue

                            switch (item.PropertyType.ToString())
                            {
                                case "System.String":
                                    item.SetValue(note, Convert.ToString(value), null);
                                    break;

                                case "System.Int32":
                                    item.SetValue(note, Convert.ToInt32(value), null);
                                    break;

                                case "System.Int64":
                                    item.SetValue(note, Convert.ToInt64(value), null);
                                    break;

                                case "System.DateTime":
                                    item.SetValue(note, Convert.ToDateTime(value), null);
                                    break;

                                case "System.Boolean":
                                    item.SetValue(note, Convert.ToBoolean(value), null);
                                    break;

                                case "System.Double":
                                    item.SetValue(note, Convert.ToDouble(value), null);
                                    break;

                                case "System.Decimal":
                                    item.SetValue(note, Convert.ToDecimal(value), null);
                                    break;

                                default:
                                    item.SetValue(note, value, null);
                                    break;
                            }

                            #endregion SetValue
                        }
                        catch
                        {
                            //throw (new Exception(ex.Message));
                        }
                    }

                    list.Add(note as T);
                }
                await reader.CloseAsync();
                await conn.CloseAsync();
                return list;
            }
            return list;
        }

        public async Task<List<T>> ExecuteSqlToListAsync<T>(string sql, [MaybeNull] params object[] parameters)
            where T : class, new()
        {
            //创建SqlCommand
            var command = await CreateCommand(sql, parameters);
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            if (conn.State == ConnectionState.Open)
            {
                var reader = await command.ExecuteReaderAsync();
                List<T> list = new();
                Type type = typeof(T);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var note = Activator.CreateInstance(type);
                        var columns = reader.GetColumnSchema();
                        foreach (var item in type.GetProperties())
                        {
                            if (!columns.Any(x => x.ColumnName.ToLower() == item.Name.ToLower())) continue;
                            var value = reader[item.Name];
                            if (!item.CanWrite || value is DBNull || value == DBNull.Value) continue;
                            try
                            {
                                #region SetValue

                                switch (item.PropertyType.ToString())
                                {
                                    case "System.String":
                                        item.SetValue(note, Convert.ToString(value), null);
                                        break;

                                    case "System.Int32":
                                    case "System.Nullable`1[System.Int32]":
                                        item.SetValue(note, Convert.ToInt32(value), null);
                                        break;

                                    case "System.Int64":
                                    case "System.Nullable`1[System.Int64]":
                                        item.SetValue(note, Convert.ToInt64(value), null);
                                        break;

                                    case "System.DateTime":
                                    case "System.Nullable`1[System.DateTime]":
                                        item.SetValue(note, Convert.ToDateTime(value), null);
                                        break;

                                    case "System.Boolean":
                                    case "System.Nullable`1[System.Boolean]":
                                        item.SetValue(note, Convert.ToBoolean(value), null);
                                        break;

                                    case "System.Double":
                                    case "System.Nullable`1[System.Double]":
                                        item.SetValue(note, Convert.ToDouble(value), null);
                                        break;

                                    case "System.Decimal":
                                    case "System.Nullable`1[System.Decimal]":
                                        item.SetValue(note, Convert.ToDecimal(value), null);
                                        break;

                                    default:
                                        item.SetValue(note, value, null);
                                        break;
                                }

                                #endregion SetValue
                            }
                            catch
                            {
                                //throw (new Exception(ex.Message));
                            }
                        }

                        list.Add(note as T);
                    }
                    await reader.CloseAsync();
                    await conn.CloseAsync();
                    return list;
                }
                return list;
            }
            return new List<T>();
        }

        public async Task<T> ExecuteSqlToEntityAsync<T>(string sql, [MaybeNull] params object[] parameters)
            where T : class, new()
        {
            //创建SqlCommand
            var command = await CreateCommand(sql, parameters);
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            if (conn.State == ConnectionState.Open)
            {
                var reader = await command.ExecuteReaderAsync();
                T entity = new();
                Type type = typeof(T);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var note = Activator.CreateInstance(type);
                        var columns = reader.GetColumnSchema();
                        foreach (var item in type.GetProperties())
                        {
                            if (!columns.Any(x => x.ColumnName.ToLower() == item.Name.ToLower())) continue;
                            var value = reader[item.Name];
                            if (!item.CanWrite || value is DBNull || value == DBNull.Value) continue;
                            try
                            {
                                #region SetValue

                                switch (item.PropertyType.ToString())
                                {
                                    case "System.String":
                                        item.SetValue(note, Convert.ToString(value), null);
                                        break;

                                    case "System.Int32":
                                    case "System.Nullable`1[System.Int32]":
                                        item.SetValue(note, Convert.ToInt32(value), null);
                                        break;

                                    case "System.Int64":
                                    case "System.Nullable`1[System.Int64]":
                                        item.SetValue(note, Convert.ToInt64(value), null);
                                        break;

                                    case "System.DateTime":
                                    case "System.Nullable`1[System.DateTime]":
                                        item.SetValue(note, Convert.ToDateTime(value), null);
                                        break;

                                    case "System.Boolean":
                                    case "System.Nullable`1[System.Boolean]":
                                        item.SetValue(note, Convert.ToBoolean(value), null);
                                        break;

                                    case "System.Double":
                                    case "System.Nullable`1[System.Double]":
                                        item.SetValue(note, Convert.ToDouble(value), null);
                                        break;

                                    case "System.Decimal":
                                    case "System.Nullable`1[System.Decimal]":
                                        item.SetValue(note, Convert.ToDecimal(value), null);
                                        break;

                                    default:
                                        item.SetValue(note, value, null);
                                        break;
                                }

                                #endregion SetValue
                            }
                            catch
                            {
                                //throw (new Exception(ex.Message));
                            }
                        }

                        entity = note as T;
                    }
                    await reader.CloseAsync();
                    await conn.CloseAsync();
                    return entity;
                }
                return entity;
            }
            return new T();
        }

        public async Task<int> ExecuteSqlToCountAsync(string sql, [MaybeNull] params object[] parameters)
        {
            //创建SqlCommand
            var command = await CreateCommand(sql, parameters);
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            if (conn.State == ConnectionState.Open)
            {
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var columns = reader.GetColumnSchema();
                        var value = reader[0];
                        if (value is int)
                        {
                            return (int)value;
                        }
                    }
                    await reader.CloseAsync();
                    await conn.CloseAsync();
                    return 0;
                }
                return 0;
            }
            return 0;
        }

        public async Task<long> ExecuteSqlToCountLongAsync(string sql, [MaybeNull] params object[] parameters)
        {
            //创建SqlCommand
            var command = await CreateCommand(sql, parameters);
            var conn = _context.Database.GetDbConnection();
            var result = 0L;
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            if (conn.State == ConnectionState.Open)
            {
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var value = reader[0];
                        if (value is long)
                        {
                            result = (long)value;
                        }
                    }
                }
                await reader.CloseAsync();
                await conn.CloseAsync();
            }
            return result;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
            => await _context.Database.BeginTransactionAsync(isolationLevel);
    }
}