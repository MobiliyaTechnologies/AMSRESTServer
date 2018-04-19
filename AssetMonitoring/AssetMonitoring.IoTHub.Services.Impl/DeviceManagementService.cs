namespace AssetMonitoring.IoTHub.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Utilities;
    using Microsoft.Azure.Devices;
    using Microsoft.Azure.Devices.Common.Exceptions;
    using System.Linq;

    public class DeviceManagementService : IDeviceManagementService
    {
        private readonly RegistryManager registryManager = null;

        public DeviceManagementService()
        {
            this.registryManager = RegistryManager.CreateFromConnectionString(ApplicationConfiguration.IotHubConnectionString);
        }

        async Task<IotHubDevice> Services.IDeviceManagementService.AddDevice(IotHubDevice iotHubDevice)
        {
            Device device;
            try
            {
                var inDevice = new Device(iotHubDevice.Id);
                inDevice.Status = DeviceStatus.Disabled;

                device = await this.registryManager.AddDeviceAsync(inDevice);
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await this.registryManager.GetDeviceAsync(iotHubDevice.Id);
            }

            iotHubDevice.Authentication = device.Authentication.SymmetricKey.PrimaryKey;
            return iotHubDevice;
        }

        async Task Services.IDeviceManagementService.DeleteDevice(string deviceId)
        {
            try
            {
                await this.registryManager.RemoveDeviceAsync(deviceId);
            }
            catch (DeviceNotFoundException)
            {

            }
        }

        async Task Services.IDeviceManagementService.EnableDevice(string deviceId)
        {
            var device = await this.registryManager.GetDeviceAsync(deviceId);

            if (device == null)
            {
                throw new InvalidOperationException(string.Format("Device with key {0} not register on IoT Hub.", deviceId));
            }

            if (device.Status == DeviceStatus.Disabled)
            {
                device.Status = DeviceStatus.Enabled;
                await this.registryManager.UpdateDeviceAsync(device);
            }
        }

        async Task<List<string>> IDeviceManagementService.GetAllConnectedDevice()
        {
            var deivces = await this.registryManager.GetDevicesAsync(int.MaxValue);
            return deivces.Where(d => d.ConnectionState == DeviceConnectionState.Connected).Select(d => d.Id).ToList();
        }

        async Task IDeviceManagementService.UpdateDeviceMetadata(string deviceId, Dictionary<string, string> details)
        {
            var deviceTwin = await this.registryManager.GetTwinAsync(deviceId);

            if (deviceTwin == null)
            {
                throw new InvalidOperationException(string.Format("Device with key {0} not register on IoT Hub.", deviceId));
            }

            foreach (var detail in details)
            {
                if (!string.IsNullOrWhiteSpace(detail.Key) && !string.IsNullOrWhiteSpace(detail.Value))
                {
                    deviceTwin.Tags[detail.Key] = detail.Value;
                }
            }

            await this.registryManager.UpdateTwinAsync(deviceId, deviceTwin, deviceTwin.ETag);
        }

        async Task<Contracts.IotHubContract.DeviceStatus> IDeviceManagementService.GetDeviceStatus(string deviceId)
        {
            var device = await this.registryManager.GetDeviceAsync(deviceId);
            return new Contracts.IotHubContract.DeviceStatus { IsEnabledDevice = device.Status == DeviceStatus.Enabled, IsConnectedDevice = device.ConnectionState == DeviceConnectionState.Connected };
        }
    }
}
