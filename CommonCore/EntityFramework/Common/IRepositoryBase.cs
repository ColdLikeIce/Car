using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    public interface IRepositoryBase<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        #region Qurey

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        IQueryable<TEntity> Query();

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <param name="propertySelectors"></param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        IQueryable<TEntity> QueryIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// Used to query a array of entities from datatable
        /// </summary>
        /// <returns>Array of entities</returns>
        Task<TEntity[]> QueryArrayAsync();

        /// <summary>
        /// Used to query a array of entities from datatable by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Array of entities</returns>
        //Task<TEntity[]> QueryArrayAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Used to query a list of entities from datatable
        /// </summary>
        /// <returns>List of entities</returns>
        Task<List<TEntity>> QueryListAsync();

        /// <summary>
        /// Used to query a list of entities from datatable by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>List of entities</returns>
        //Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Used to query a single entity from datatable by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets an entity with given given predicate or null if not found.
        /// </summary>
        /// <param name="predicate">Predicate to filter entities</param>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null);

        #endregion Qurey

        #region Insert

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="submit"></param>
        /// <returns>Inserted entity</returns>
        Task<TEntity> InsertAsync(TEntity entity, bool submit = true);

        Task<List<TEntity>> BatchInsertAsync(List<TEntity> entities, bool submit = true);

        #endregion Insert

        #region Update

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="submit"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity, bool submit = true);

        Task<TEntity> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>> expression, bool submit = true);

        Task BatchUpdateAsync(List<TEntity> entities, bool submit = true);

        Task BatchUpdateAsync(List<TEntity> entities, Expression<Func<TEntity, object>> expression, bool submit = true);

        #endregion Update

        #region Delete

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="submit"></param>
        /// <returns>Entity to be deleted</returns>
        Task DeleteAsync(TEntity entity, bool submit = true);

        Task BatchDeleteAsync(List<TEntity> entities, bool submit = true);

        #endregion Delete

        #region Expression

        TDbContext Context { get; }

        DbSet<TEntity> Table { get; }

        void Attach(TEntity entity);

        #endregion Expression
    }
}