namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class SensorCapabilityFilter : Entity
    {
        public SensorCapabilityFilter()
        {
            this.SensorRules = new HashSet<SensorRule>();
        }

        public override int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Operator { get; set; }

        public string MinValue { get; set; }

        public string MaxValue { get; set; }

        [ForeignKey("Capability")]
        public int? CapabilityId { get; set; }

        public virtual Capability Capability { get; set; }

        public virtual ICollection<SensorRule> SensorRules { get; set; }
    }
}
