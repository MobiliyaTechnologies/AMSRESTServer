namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;

    public class IotHubGateway
    {
        public string IotHubHostName { get; set; }

        public string IotHubAccessTocken { get; set; }

        public string DeviceConnectionString { get; set; }

        public List<Capability> Capabilities { get; set; }
    }
}
