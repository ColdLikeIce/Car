using Dapper;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Xml.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HeyTripCarWeb.Db
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnection _dbConnection;

        public DapperRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var tableName = await GetTableName<T>();
            var query = $"SELECT * FROM {tableName}";
            var queryData = await _dbConnection.QueryAsync<T>(query);
            return queryData.ToList();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            var tableName = await GetTableName<T>();
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(p => IsKeyProperty(p));
            var query = $"SELECT * FROM {tableName} WHERE {propertyInfo.Name} = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task<T> GetByIdAsync(string sql, object par)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, par);
        }

        public async Task<List<T>> GetListBySqlAsync(string sql, object par)
        {
            var queryData = await _dbConnection.QueryAsync<T>(sql, par);
            return queryData.ToList();
        }

        public async Task<int> UpdateBySqlAsync(string sql, object par)
        {
            return await _dbConnection.ExecuteAsync(sql, par);
        }

        public async Task<int> ExecuteSql(string sql, object par)
        {
            return await _dbConnection.ExecuteAsync(sql, par);
        }

        public async Task<string> GetTableName<T>()
        {
            var tableType = typeof(T);
            TableAttribute xmlRootAttr = (TableAttribute)tableType.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            return xmlRootAttr.Name;
        }

        public async Task<int> InsertAsync(T entity)
        {
            var tableName = await GetTableName<T>();
            var query = GenerateInsertQuery(tableName, entity);
            return await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task<int> InsertOrUpdate(T entity, string ignoreStr = "")
        {
            var tableName = await GetTableName<T>();
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(p => IsKeyProperty(p));
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(entity);
                if (value != null && value.ToString() != "0")
                {
                    await UpdateAsync(entity, ignoreStr);
                }
                else
                {
                    await InsertAsync(entity);
                }
            }
            return 1;
        }

        public async Task<int> UpdateAsync(T entity, string ignoreStr = "")
        {
            var tableName = await GetTableName<T>();
            var query = GenerateUpdateQuery(tableName, entity, ignoreStr);
            return await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task<int> DeleteAsync(object id)
        {
            var tableName = typeof(T).Name;
            var query = $"DELETE FROM {tableName} WHERE Id = @Id";
            return await _dbConnection.ExecuteAsync(query, new { Id = id });
        }

        private string GenerateInsertQuery(string tableName, T entity)
        {
            var properties = typeof(T).GetProperties().Where(p => !IsIdentityProperty(p)) // 排除自增属性
                                   .ToArray();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));
            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }

        /// <summary>
        /// 排除自增属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool IsIdentityProperty(PropertyInfo property)
        {
            var identityAttribute = property.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute>();
            return identityAttribute != null && identityAttribute.DatabaseGeneratedOption == System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity;
        }

        /// <summary>
        /// 排除Key属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool IsKeyProperty(PropertyInfo property)
        {
            return property.GetCustomAttributes<KeyAttribute>().Any();
        }

        /// <summary>
        /// 更新主键
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GenerateUpdateQuery(string tableName, T entity, string ignoreStr)
        {
            var properties = typeof(T).GetProperties();
            var key = properties.Where(p => IsKeyProperty(p) || IsIdentityProperty(p)) // 排除自增属性
                               .FirstOrDefault();
            properties = properties.Where(p => !IsKeyProperty(p) && !IsIdentityProperty(p)) // 排除自增属性
                               .ToArray();
            if (!string.IsNullOrWhiteSpace(ignoreStr))
            {
                var ignore = ignoreStr.Split(",").ToList();
                if (ignore != null && ignore.Count() > 0)
                {
                    properties = properties.Where(n => !ignore.Contains(n.Name)).ToArray();
                }
            }

            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {tableName} SET {setClause} WHERE {key.Name} = @{key.Name}";
        }
    }
}