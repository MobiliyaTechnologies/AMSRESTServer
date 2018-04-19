namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class SensorRule : Entity
    {
        public override int Id { get; set; }

        public string MinThreshold { get; set; }

        public string MaxThreshold { get; set; }

        [ForeignKey("CapabilityFilter")]
        public int? CapabilityFilterId { get; set; }

        public virtual SensorCapabilityFilter CapabilityFilter { get; set; }

        [ForeignKey("SensorGroup")]
        public int? SensorGroupId { get; set; }

        public virtual SensorGroup SensorGroup { get; set; }
    }
}
