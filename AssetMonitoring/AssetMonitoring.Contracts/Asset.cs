namespace AssetMonitoring.Contracts
{
    using System.ComponentModel.DataAnnotations;

    public class Asset
    {
        [Required]
        public string AssetBarcode { get; set; }

        [Required]
        public string SensorKey { get; set; }
    }
}
