namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;

    public class GatewaytAsset
    {
        public string AssetBarcode { get; set; }

        public bool IsDamagedAsset { get; set; }

        public List<SensorCapability> SensorCapabilities { get; set; }
    }
}
