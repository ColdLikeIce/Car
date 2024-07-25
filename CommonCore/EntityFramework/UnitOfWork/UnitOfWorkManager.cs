using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CommonCore.EntityFramework.UnitOfWork
{
    public class UnitOfWorkManager<TDbContext> : IUnitOfWorkManager
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public UnitOfWorkManager(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IUnitOfWorkCompleteHandle Begin()
        {
            var tran = _dbContext.Database.BeginTransaction();
            var handle = new UnitOfWorkCompleteHandle<TDbContext>(tran);
            return handle;
        }

        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions { Scope = scope });
        }

        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            var scope = new TransactionScope(
                options.Scope.GetValueOrDefault(),
                new TransactionOptions
                {
                    Timeout = options.Timeout.GetValueOrDefault(),
                    IsolationLevel = options.IsolationLevel.GetValueOrDefault()
                });
            var handle = new UnitOfWorkCompleteScopeHandle<TDbContext>(scope);
            return handle;
        }
    }
}