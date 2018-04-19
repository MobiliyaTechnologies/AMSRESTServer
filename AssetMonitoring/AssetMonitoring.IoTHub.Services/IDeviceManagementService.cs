
namespace AssetMonitoring.IoTHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.IotHubContract;

    /// <summary>
    /// Provides IoT HUB device management operations.
    /// </summary>
    public interface IDeviceManagementService
    {
        /// <summary>
        /// Adds the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns>The added device.</returns>
        Task<IotHubDevice> AddDevice(IotHubDevice device);

        /// <summary>
        /// Deletes the device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        Task DeleteDevice(string deviceId);

        /// <summary>
        /// Enables the device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        Task EnableDevice(string deviceId);

        /// <summary>
        /// Create or update the device meta-data.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="details">The details, dictionary of meta-data name and it's value..</param>
        Task UpdateDeviceMetadata(string deviceId, Dictionary<string, string> details);

        /// <summary>
        /// Gets all devices connected to IoT hub.
        /// </summary>
        /// <returns>The device ids.</returns>
        Task<List<string>> GetAllConnectedDevice();

        /// <summary>
        /// Gets the device status for given identifier.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device status.</returns>
        Task<DeviceStatus> GetDeviceStatus(string deviceId);
    }
}
