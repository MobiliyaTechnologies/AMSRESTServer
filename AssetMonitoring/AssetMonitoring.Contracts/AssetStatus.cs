namespace AssetMonitoring.Contracts
{
    using System;

    public class AssetStatus
    {
        public string Capability { get; set; }

        public string CapabilityFilter { get; set; }

        public string MinThreashold { get; set; }

        public string MaxThreashold { get; set; }

        public DateTime Timestamp { get; set; }

        public string Value { get; set; }
    }
}
