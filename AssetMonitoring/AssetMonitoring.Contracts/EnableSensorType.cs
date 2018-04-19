namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;

    public class EnableSensorType
    {
        public EnableSensorType()
        {
            this.SensorCapabilities = new List<SensorCapability>();
        }

        public List<SensorCapability> SensorCapabilities { get; set; }

        public string SensorType { get; set; }
    }
}
