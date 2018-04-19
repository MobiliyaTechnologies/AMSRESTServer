namespace AssetMonitoring.Components.Repository.EntityFramework
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using AssetMonitoring.Components.Context;
    using AssetMonitoring.Components.Transaction;

    /// <summary>
    /// Implementation of <see cref="IRepository"/> that defines data access against NHibernate.
    /// </summary>
    /// <remarks>
    /// Light wrapper around <seealso cref="DBContext"/> to target specific functionality and remove others. 
    /// </remarks>
    public class DbContextRepository : IRepository, ITransactionHandler, IDisposable
    {
        private readonly DbContext dbContext = null;
        private readonly IContextInfoProvider context = null;

        private DbContextTransaction dbTransaction = null;

        public DbContextRepository(ITransaction transaction, DbContext dbContext, IContextInfoProvider context)
        {
            transaction.RegisterTransactionHandler(this);
            this.dbContext = dbContext;
            this.context = context;
        }

        T IRepository.Read<T>(int entityId)
        {
            return this.dbContext.Set<T>().FirstOrDefault(e => e.Id == entityId);
        }

        IQueryable<T> IRepository.Query<T>()
        {
            return this.dbContext.Set<T>();
        }

        void IRepository.Persist<T>(T entity)
        {
            if (entity.Id != 0)
            {
                entity.ModifiedBy = this.context.Current.UserId;
                entity.ModifiedOn = DateTime.Now;

                this.dbContext.Entry<T>(entity).State = EntityState.Modified;
            }
            else
            {
                if (this.context.Current != null)
                {
                    entity.CreatedBy = this.context.Current.UserId;
                    entity.CreatedOn = DateTime.Now;
                }

                this.dbContext.Set<T>().Add(entity);
            }
        }

        void IRepository.Delete<T>(T entity)
        {
            this.dbContext.Set<T>().Remove(entity);
        }

        void IRepository.Flush()
        {
            this.dbContext.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            (this.dbContext as IDisposable).Dispose();
        }

        void ITransactionHandler.TransactionBegin(IsolationLevel isolationLevel)
        {
            if (this.dbTransaction != null)
            {
                throw new InvalidOperationException("Cannot start multiple transactions");
            }

            this.dbTransaction = this.dbContext.Database.BeginTransaction(isolationLevel);
        }

        void ITransactionHandler.TransactionCommit()
        {
            if (this.dbTransaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started");
            }

            try
            {
                this.dbContext.SaveChanges();
                this.dbTransaction.Commit();
            }
            finally { this.dbTransaction = null; }
        }

        void ITransactionHandler.TransactionRollback()
        {
            if (this.dbTransaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started");
            }

            try { this.dbTransaction.Rollback(); }
            finally { this.dbTransaction = null; }
        }
    }
}
