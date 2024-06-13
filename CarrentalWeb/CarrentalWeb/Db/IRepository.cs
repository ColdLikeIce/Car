namespace CarrentalWeb.Db
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<int> InsertAsync(T entity);

        Task<int> UpdateAsync(T entity);

        Task<bool> DeleteAsync(int id);
    }
}