using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.UnitOfWork
{
    /// <summary>
    /// Used to complete a unit of work. This interface can not be injected or directly used.
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        /// <summary>
        /// Completes this unit of work. It saves all changes and commit transaction if exists.
        /// </summary>
        void Complete();

        /// <summary>
        /// Completes this unit of work. It saves all changes and commit transaction if exists.
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();

        /// <summary>
        ///
        /// </summary>
        void Rollback();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task RollbackAsync();
    }
}