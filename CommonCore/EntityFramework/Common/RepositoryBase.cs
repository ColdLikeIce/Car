using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    public class RepositoryBase<TDbContext, TEntity> : IRepositoryBase<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        /// <summary>
        ///
        /// </summary>
        public TDbContext Context { get; }

        /// <summary>
        ///
        /// </summary>
        public DbSet<TEntity> Table => Context.Set<TEntity>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public RepositoryBase(TDbContext context)
        {
            Context = context;
        }

        #region Qurey

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public IQueryable<TEntity> Query()
        {
            if (!(Context.Set<TEntity>() is IQueryable<TEntity> query))
                throw new Exception($"{typeof(TEntity)} TEntity cannot be empty！");
            return query;
        }

        /// <summary>
        /// Used to get a IQueryable that is used to retrieve entities from entire table.
        /// </summary>
        /// <param name="propertySelectors"></param>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public IQueryable<TEntity> QueryIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors == null || !propertySelectors.Any())
            {
                return Query();
            }

            var query = Query();

            return propertySelectors.Aggregate(query, (current, propertySelector) => current.Include(propertySelector));
        }

        /// <summary>
        /// Used to query a array of entities from data table
        /// </summary>
        /// <returns>Array of entities</returns>
        public async Task<TEntity[]> QueryArrayAsync()
        {
            return await Query().ToArrayAsync();
        }

        /// <summary>
        /// Used to query a array of entities from data table by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Array of entities</returns>
        public async Task<TEntity[]> QueryArrayAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Query().Where(predicate).ToArrayAsync();
        }

        /// <summary>
        /// Used to query a list of entities from data table
        /// </summary>
        /// <returns>List of entities</returns>
        public async Task<List<TEntity>> QueryListAsync()
        {
            return await Query().ToListAsync();
        }

        /// <summary>
        /// Used to query a list of entities from data table by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>List of entities</returns>
        public async Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return await Query().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Used to query a single entity from datatable by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await Query().SingleAsync();
            }
            return await Query().SingleAsync(predicate);
        }

        /// <summary>
        /// Gets an entity with given given predicate or null if not found.
        /// </summary>
        /// <param name="predicate">Predicate to filter entities</param>
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await Query().FirstOrDefaultAsync();
            }
            return await Query().FirstOrDefaultAsync(predicate);
        }

        #endregion Qurey

        #region Insert

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="submit"></param>
        /// <returns>Inserted entity</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity, bool submit = true)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Added;
            if (submit)
            {
                await Context.SaveChangesAsync();
            }

            return entity;
        }

        public virtual async Task<List<TEntity>> BatchInsertAsync(List<TEntity> entities, bool submit = true)
        {
            foreach (var entity in entities)
            {
                AttachIfNot(entity);
                Context.Entry(entity).State = EntityState.Added;
            }
            if (submit)
            {
                await Context.SaveChangesAsync();
            }

            return entities;
        }

        #endregion Insert

        #region Update

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="submit"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool submit = true)
        {
            Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            if (submit)
            {
                await Context.SaveChangesAsync();
            }
            return await Task.FromResult(entity);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>> expression, bool submit = true)
        {
            Attach(entity);
            var entry = Context.Entry(entity);
            //entry.State = EntityState.Unchanged;
            foreach (var proInfo in expression.GetPropertyAccessList())
            {
                if (!string.IsNullOrEmpty(proInfo.Name))
                    entry.Property(proInfo.Name).IsModified = true;
            }
            if (submit)
            {
                await Context.SaveChangesAsync();
            }
            return await Task.FromResult(entity);
        }

        public virtual async Task BatchUpdateAsync(List<TEntity> entities, bool submit = true)
        {
            foreach (var entity in entities)
            {
                Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
            if (submit)
            {
                await Context.SaveChangesAsync();
            }
        }

        public virtual async Task BatchUpdateAsync(List<TEntity> entities, Expression<Func<TEntity, object>> expression, bool submit = true)
        {
            foreach (var entity in entities)
            {
                Attach(entity);
                var entry = Context.Entry(entity);
                //entry.State = EntityState.Unchanged;
                foreach (var proInfo in expression.GetPropertyAccessList())
                {
                    if (!string.IsNullOrEmpty(proInfo.Name))
                        //4.4将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                        entry.Property(proInfo.Name).IsModified = true;
                }
            }
            if (submit)
            {
                await Context.SaveChangesAsync();
            }
        }

        #endregion Update

        #region Delete

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="submit"></param>
        /// <returns>Entity to be deleted</returns>
        public virtual async Task DeleteAsync(TEntity entity, bool submit = true)
        {
            AttachIfNot(entity);
            await Task.FromResult(Table.Remove(entity));
            if (submit)
            {
                await Context.SaveChangesAsync();
            }
        }

        public virtual async Task BatchDeleteAsync(List<TEntity> entities, bool submit = true)
        {
            foreach (var entity in entities)
            {
                AttachIfNot(entity);
                Context.Entry(entity).State = EntityState.Deleted;
            }
            Table.RemoveRange(entities);
            if (submit)
            {
                await Context.SaveChangesAsync();
            }
        }

        #endregion Delete

        #region Expression

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        public void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }

        public void Attach(TEntity entity)
        {
            Table.Attach(entity);
        }

        #endregion Expression
    }
}