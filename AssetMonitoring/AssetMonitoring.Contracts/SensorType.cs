namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SensorType
    {
        public SensorType()
        {
            this.Capabilities = new List<Capability>();
            this.CapabilityIds = new List<int>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Capability> Capabilities { get; set; }

        public List<int> CapabilityIds { get; set; }
    }
}