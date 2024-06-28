namespace HeyTripCarWeb.Db
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T> GetByIdAsync(object id);

        Task<int> InsertAsync(T entity);

        Task<int> UpdateAsync(T entity);

        Task<int> DeleteAsync(object id);

        #region sql封装

        Task<T> GetByIdAsync(string sql, object par);

        Task<int> UpdateBySqlAsync(string sql, object par);

        Task<int> ExecuteSql(string sql, object par);

        #endregion sql封装
    }
}