namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using AssetMonitoring.Components.Domain;

    public class Capability : Entity
    {
        public Capability()
        {
            this.SensorTypes = new HashSet<SensorType>();
            this.SensorCapabilityFilters = new HashSet<SensorCapabilityFilter>();
        }

        public override int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Unit { get; set; }

        public string SupportedUnits { get; set; }

        public virtual ICollection<SensorType> SensorTypes { get; set; }

        public virtual ICollection<SensorCapabilityFilter> SensorCapabilityFilters { get; set; }
    }
}
