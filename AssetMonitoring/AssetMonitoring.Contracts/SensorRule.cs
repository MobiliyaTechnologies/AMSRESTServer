namespace AssetMonitoring.Contracts
{
    using System.ComponentModel.DataAnnotations;

    public class SensorRule
    {
        public int Id { get; set; }

        [Required]
        public string MinThreshold { get; set; }

        public string MaxThreshold { get; set; }

        [Required]
        public int CapabilityFilterId { get; set; }

        public int SensorGroupId { get; set; }

        public string Capability { get; set; }

        public string SensorGroupName { get; set; }

        public string Operator { get; set; }

        public bool RuleCreationInProgress { get; set; }
    }
}