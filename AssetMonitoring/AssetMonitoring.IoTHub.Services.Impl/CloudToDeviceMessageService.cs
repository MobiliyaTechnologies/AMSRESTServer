namespace AssetMonitoring.IoTHub.Services.Impl
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading.Tasks;
    using AssetMonitoring.Utilities;
    using Microsoft.Azure.Devices;
    using Newtonsoft.Json;

    public sealed class CloudToDeviceMessageService : ICloudToDeviceMessageService
    {
        private readonly ServiceClient serviceClient;
        private readonly IDeviceManagementService deviceManagementService;

        public CloudToDeviceMessageService(IDeviceManagementService deviceManagementService)
        {
            this.deviceManagementService = deviceManagementService;
            this.serviceClient = ServiceClient.CreateFromConnectionString(ApplicationConfiguration.IotHubConnectionString);
        }

        async Task ICloudToDeviceMessageService.SendDeviceMessage<T>(string deviceId, T message, IDictionary<string, string> messageProperties, bool allowDisconnectedDevice)
        {
            var deviceStatus = await this.deviceManagementService.GetDeviceStatus(deviceId);
            if (deviceStatus.IsEnabledDevice && (allowDisconnectedDevice || deviceStatus.IsConnectedDevice))
            {
                var jsonMessage = JsonConvert.SerializeObject(message);

                using (var deviceMessage = new Message(Encoding.ASCII.GetBytes(jsonMessage)))
                {
                    if (messageProperties != null)
                    {
                        foreach (var messageProperty in messageProperties)
                        {
                            deviceMessage.Properties.Add(messageProperty.Key, messageProperty.Value);
                        }
                    }

                    await this.serviceClient.SendAsync(deviceId, deviceMessage);
                }
            }
        }
    }
}
