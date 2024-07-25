using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.UnitOfWork
{
    public class UnitOfWorkCompleteHandle<TDbContext> : IUnitOfWorkCompleteHandle
        where TDbContext : DbContext
    {
        private readonly IDbContextTransaction _dbContextTransaction;

        public UnitOfWorkCompleteHandle(IDbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction ?? throw new ArgumentNullException(nameof(dbContextTransaction));
        }

        public void Complete()
        {
            _dbContextTransaction.Commit();
        }

        public async Task CompleteAsync()
        {
            await _dbContextTransaction.CommitAsync();
        }

        public void Rollback()
        {
            _dbContextTransaction.Rollback();
        }

        public async Task RollbackAsync()
        {
            await _dbContextTransaction.RollbackAsync();
        }

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }
    }
}