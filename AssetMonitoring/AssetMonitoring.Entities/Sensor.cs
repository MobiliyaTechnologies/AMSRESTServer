namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class Sensor : Entity
    {
        public override int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SensorKey { get; set; }

        [ForeignKey("SensorType")]
        public int? SensorTypeId { get; set; }

        public virtual SensorType SensorType { get; set; }

        [ForeignKey("SensorGroup")]
        public int? SensorGroupId { get; set; }

        public virtual SensorGroup SensorGroup { get; set; }

        [ForeignKey("Asset")]
        public int? AssetId { get; set; }

        public virtual Asset Asset { get; set; }
    }
}
