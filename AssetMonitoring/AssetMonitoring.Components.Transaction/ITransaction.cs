using System.Data;

namespace AssetMonitoring.Components.Transaction
{
    public interface ITransaction
    {
        void BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();

        void RegisterTransactionHandler(ITransactionHandler transactionHandler);
    }
}
