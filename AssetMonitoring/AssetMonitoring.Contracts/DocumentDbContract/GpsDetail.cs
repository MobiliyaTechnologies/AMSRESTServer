namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System;
    using System.Collections.Generic;

    public class GpsDetail
    {
        public DateTime Timestamp { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
