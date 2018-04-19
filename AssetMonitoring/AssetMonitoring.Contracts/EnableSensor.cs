namespace AssetMonitoring.Contracts
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class EnableSensor
    {
        public string SensorKey { get; set; }

        public string SensorType { get; set; }

        public int GroupId { get; set; }

        public string AssetBarcode { get; set; }
    }
}
