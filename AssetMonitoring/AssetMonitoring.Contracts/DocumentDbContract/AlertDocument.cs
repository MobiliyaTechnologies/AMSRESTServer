namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System;

    public class AlertDocument
    {
        public string value { get; set; }

        public string capability { get; set; }

        public string assetbarcode { get; set; }

        public int sensorruleid { get; set; }

        public DateTime timestamp { get; set; }

        public int groupid { get; set; }

        public long _ts { get; set; }
    }
}
