namespace AssetMonitoring.Components.Transaction.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class Transaction : ITransaction
    {
        private readonly List<ITransactionHandler> transactionHandlers = new List<ITransactionHandler>();
        private bool transactionActive = false;
        private IsolationLevel isolationLevel = IsolationLevel.Unspecified;

        void ITransaction.BeginTransaction(IsolationLevel isolationLevel)
        {
            if (transactionActive)
                throw new InvalidOperationException("Cannot begin an already active transaction");

            transactionActive = true;
            this.isolationLevel = isolationLevel;

            foreach (var handler in transactionHandlers)
            {
                if (handler != null)
                {
                    handler.TransactionBegin(this.isolationLevel);
                }
            }
        }

        void ITransaction.CommitTransaction()
        {
            if (!transactionActive)
                throw new InvalidOperationException("Cannot commit a non-active transaction");

            transactionActive = false;
            isolationLevel = IsolationLevel.Unspecified;

            foreach (var handler in transactionHandlers)
            {
                if (handler != null)
                {
                    handler.TransactionCommit();
                }
            }
        }

        void ITransaction.RollbackTransaction()
        {
            if (!transactionActive)
                throw new InvalidOperationException("Cannot rollback a non-active transaction");

            transactionActive = false;
            isolationLevel = IsolationLevel.Unspecified;

            foreach (var handler in transactionHandlers)
            {
                if (handler != null)
                {
                    handler.TransactionRollback();
                }
            }
        }

        void ITransaction.RegisterTransactionHandler(ITransactionHandler transactionHandler)
        {
            if (transactionActive)
                transactionHandler.TransactionBegin(isolationLevel);

            transactionHandlers.Add(transactionHandler);
        }
    }
}
