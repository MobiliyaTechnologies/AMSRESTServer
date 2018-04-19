namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using AssetMonitoring.Components.Domain;

    public class SensorType : Entity
    {
        public SensorType()
        {
            this.Capabilities = new HashSet<Capability>();
            this.Sensors = new HashSet<Sensor>();
        }

        public override int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Capability> Capabilities { get; set; }

        public virtual ICollection<Sensor> Sensors { get; set; }
    }
}
