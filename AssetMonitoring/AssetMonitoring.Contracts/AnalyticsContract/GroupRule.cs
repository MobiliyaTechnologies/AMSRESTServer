namespace AssetMonitoring.Contracts.AnalyticsContract
{
    using System.ComponentModel.DataAnnotations;

    public class GroupRule
    {
        [Required]
        public string MinThreshold { get; set; }

        public string MaxThreshold { get; set; }

        [Required]
        public int RuleId { get; set; }

        [Required]
        public string Operator { get; set; }

        [Required]
        public int CapabilityId { get; set; }

        [Required]
        public string Filter { get; set; }
    }
}