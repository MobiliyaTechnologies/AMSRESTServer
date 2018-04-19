namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using AssetMonitoring.Components.Domain;

    public class IndoorLayout : Entity
    {
        public IndoorLayout()
        {
            this.Gateways = new HashSet<Gateway>();
        }

        public override int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public virtual ICollection<Gateway> Gateways { get; set; }
    }
}
