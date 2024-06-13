using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CarrentalWeb.Db
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnection _dbConnection;

        public DapperRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string query = $"SELECT * FROM {typeof(T).Name}"; // 假设表名和实体名相同，且为复数形式
            return await _dbConnection.QueryAsync<T>(query);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            string query = $"SELECT * FROM {typeof(T).Name} WHERE Id = @Id"; // 假设表名和实体名相同，且为复数形式
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task<int> InsertAsync(T entity)
        {
            var query = GenerateInsertQuery();
            return await _dbConnection.ExecuteAsync(query, entity);
        }

        private string GenerateInsertQuery()
        {
            // Assuming "T" has properties that map to the database columns
            var properties = typeof(T).GetProperties()
                            .Where(p => !Attribute.IsDefined(p, typeof(KeyAttribute)));
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            return $"INSERT INTO {typeof(T).Name} ({columns}) VALUES ({parameters})";
        }

        private string GenerateUpdateQuery()
        {
            // Assuming "T" has an "Id" property
            var properties = typeof(T).GetProperties()
                            .Where(p => !Attribute.IsDefined(p, typeof(KeyAttribute))); // Exclude Id column
            var assignments = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {typeof(T).Name} SET {assignments} WHERE Id = @Id";
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var query = GenerateUpdateQuery();
            return await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";
            int rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}