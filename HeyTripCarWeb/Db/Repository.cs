using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Dapper.FluentMap.Dommel.Mapping;
using Dommel;
using System.Data;

namespace HeyTripCarWeb.Db
{
    public class Repository<T> where T : class
    {
        private readonly IDbConnection _connection;

        public Repository(IDbConnection connection)
        {
            _connection = connection;
            /*    FluentMapper.Initialize(config =>
                {
                    config.AddMap(new DommelEntityMap<T>());
                });*/
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _connection.GetAllAsync<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _connection.GetAsync<T>(id);
        }

        public async Task<object> InsertAsync(T entity)
        {
            return await _connection.InsertAsync(entity);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            return await _connection.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            return await _connection.DeleteAsync(entity);
        }
    }
}