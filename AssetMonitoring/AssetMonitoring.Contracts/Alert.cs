namespace AssetMonitoring.Contracts
{
    using System;

    public class Alert
    {
        public string Value { get; set; }

        public string Capability { get; set; }

        public string AssetBarcode { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
