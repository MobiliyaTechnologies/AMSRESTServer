namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System;

    public class GroupLocation
    {
        public int GroupId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; }

        public long _ts { get; set; }
    }
}
