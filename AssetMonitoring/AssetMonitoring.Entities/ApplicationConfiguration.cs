namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using AssetMonitoring.Components.Domain;

    public class ApplicationConfiguration : Entity
    {
        public ApplicationConfiguration()
        {
            this.ApplicationConfigurationEntries = new HashSet<ApplicationConfigurationEntry>();
        }

        public override int Id { get; set; }

        public string ConfigurationType { get; set; }

        public virtual ICollection<ApplicationConfigurationEntry> ApplicationConfigurationEntries { get; set; }
    }
}
