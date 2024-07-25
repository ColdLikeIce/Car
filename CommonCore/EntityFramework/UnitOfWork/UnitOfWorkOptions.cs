using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CommonCore.EntityFramework.UnitOfWork
{
    /// <summary>
    /// Unit of work options.
    /// </summary>
    public class UnitOfWorkOptions
    {
        /// <summary>
        /// Creates a new UnitOfWorkOptions object.
        /// </summary>
        public UnitOfWorkOptions()
        {
            IsTransactional = true;
            Scope = TransactionScopeOption.Required;
            Timeout = TimeSpan.FromMinutes(2);
            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
        }

        /// <summary>
        /// Scope option.
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }

        /// <summary>
        /// Is this Unit of work transactional? Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// Timeout of Unit of work As milliseconds. Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this Unit of work is transactional, this option indicated the isolation level of the transaction. Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// This option should be set to System.Transactions.TransactionScopeAsyncFlowOption.Enabled if unit of work is used in an async scope.
        /// </summary>
        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }
    }
}