namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SensorGroup
    {
        public SensorGroup()
        {
            this.Sensors = new List<Sensor>();
            this.SensorIds = new List<int>();
            this.CapabilityNames = new List<string>();
            this.SensorRules = new List<SensorRule>();
            this.AssetIds = new List<int>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Sensor> Sensors { get; set; }

        public List<int> SensorIds { get; set; }

        public List<string> CapabilityNames { get; set; }

        public List<SensorRule> SensorRules { get; set; }

        public List<GroupAsset> Assets { get; set; }

        public List<int> AssetIds { get; set; }

        public bool RuleCreationInProgress { get; set; }
    }
}
