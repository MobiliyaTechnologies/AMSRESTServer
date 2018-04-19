namespace AssetMonitoring.Services.Impl
{
    using System.Threading.Tasks;
    using AssetMonitoring.Utilities;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    public sealed class QueueStorageService : IQueueStorageService
    {
        private readonly CloudQueueClient cloudQueueClient;

        public QueueStorageService()
        {
            var storageAccount = CloudStorageAccount.Parse(ApplicationConfiguration.BlobStorageConnectionString);
            this.cloudQueueClient = storageAccount.CreateCloudQueueClient();
        }

        async Task IQueueStorageService.SendMessage<T>(string queueName, T message)
        {
            // Retrieve a reference to a container.
            CloudQueue queue = this.cloudQueueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            if (await queue.CreateIfNotExistsAsync())
            {
                var poisonQueue = this.cloudQueueClient.GetQueueReference(queueName + ApplicationConstant.PoisonQueueSuffix);
                await poisonQueue.CreateIfNotExistsAsync();
            }

            var jsonMessage = JsonConvert.SerializeObject(message);
            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(jsonMessage);
            queue.AddMessage(cloudQueueMessage);
        }
    }
}
