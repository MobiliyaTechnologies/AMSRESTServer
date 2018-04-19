namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Capability
    {
        public Capability()
        {
            this.Filters = new List<CapabilityFilter>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Unit { get; set; }

        public List<string> SupportedUnits { get; set; }

        public List<CapabilityFilter> Filters { get; set; }
    }
}