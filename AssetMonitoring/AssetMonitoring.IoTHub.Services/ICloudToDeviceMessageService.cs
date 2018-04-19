using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetMonitoring.IoTHub.Services
{
    public interface ICloudToDeviceMessageService
    {
        /// <summary>
        /// Sends the device message.
        /// </summary>
        /// <typeparam name="T">Message type to send.</typeparam>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageProperties">The message properties.</param>
        /// <param name="allowDisconnectedDevice">if it's message will be send to disconnected device also.</param>
        Task SendDeviceMessage<T>(string deviceId, T message, IDictionary<string, string> messageProperties, bool allowDisconnectedDevice = false);
    }
}
