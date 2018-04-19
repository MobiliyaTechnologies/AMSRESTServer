namespace AssetMonitoring.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides azure queue storage operations.
    /// </summary>
    public interface IQueueStorageService
    {
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        Task SendMessage<T>(string queueName, T message);
    }
}
