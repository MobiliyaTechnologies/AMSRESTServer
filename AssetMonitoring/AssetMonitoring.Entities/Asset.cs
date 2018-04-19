namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class Asset : Entity
    {
        public Asset()
        {
            this.Sensors = new HashSet<Sensor>();
        }

        public override int Id { get; set; }

        public string AssetBarcode { get; set; }

        public virtual ICollection<Sensor> Sensors { get; set; }

        [ForeignKey("SensorGroup")]
        public int? SensorGroupId { get; set; }

        public virtual SensorGroup SensorGroup { get; set; }
    }
}
