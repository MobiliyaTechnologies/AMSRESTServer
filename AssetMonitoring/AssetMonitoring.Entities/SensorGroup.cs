namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using AssetMonitoring.Components.Domain;

    public class SensorGroup : Entity
    {
        public SensorGroup()
        {
            this.Sensors = new HashSet<Sensor>();
            this.SensorRules = new HashSet<SensorRule>();
            this.Assets = new HashSet<Asset>();
        }

        public override int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Sensor> Sensors { get; set; }

        public virtual ICollection<SensorRule> SensorRules { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }

        public bool RuleCreationInProgress { get; set; }
    }
}
