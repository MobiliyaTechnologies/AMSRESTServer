using System.Data;

namespace AssetMonitoring.Components.Transaction
{
    public interface ITransactionHandler
    {
        void TransactionBegin(IsolationLevel isolationLevel);
        void TransactionCommit();
        void TransactionRollback();
    }
}
