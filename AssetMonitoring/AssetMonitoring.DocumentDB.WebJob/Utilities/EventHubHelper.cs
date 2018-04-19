namespace AssetMonitoring.DocumentDB.WebJob.Utilities
{
    using System;
    using System.Text;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    public static class EventHubHelper
    {
        private static readonly EventHubClient EventHubClient = EventHubClient.CreateFromConnectionString(ApplicationConfig.EventHubConnectionString, ApplicationConfig.EventHubName);

        public static void SendMessage<T>(T message)
        {
            string jsonMessage = null;

            try
            {
                 jsonMessage = JsonConvert.SerializeObject(message);
                EventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(jsonMessage)));
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to send message to event hub. Message - {0} .", jsonMessage);
            }
        }
    }
}
