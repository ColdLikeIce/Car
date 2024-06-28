using Dapper;
using Microsoft.Data.SqlClient;
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
            var query = $"SELECT * FROM {tableName} WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task<T> GetByIdAsync(string sql, object par)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, par);
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

        public async Task<int> UpdateAsync(T entity)
        {
            var tableName = typeof(T).Name;
            var query = GenerateUpdateQuery(tableName, entity);
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

        private string GenerateUpdateQuery(string tableName, T entity)
        {
            var properties = typeof(T).GetProperties();
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
        }
    }
}