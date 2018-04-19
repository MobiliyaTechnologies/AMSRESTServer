namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Configuration
    {
        public Configuration()
        {
            this.ApplicationConfigurationEntries = new List<ApplicationConfigurationEntry>();
        }

        [Required]
        public string ApplicationConfigurationType { get; set; }

        public List<ApplicationConfigurationEntry> ApplicationConfigurationEntries { get; set; }
    }
}
