namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;

    public class GroupAsset
    {
        public int AssetId { get; set; }

        public string AssetBarcode { get; set; }

        public string GroupName { get; set; }

        public List<string> SensorKeys { get; set; }
    }
}
