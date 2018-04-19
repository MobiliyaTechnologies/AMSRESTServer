namespace AssetMonitoring.Contracts
{
    using System.ComponentModel.DataAnnotations;

    public class ApplicationConfigurationEntry
    {
        public int Id { get; set; }

        [Required]
        public string ConfigurationKey { get; set; }

        [Required]
        public string ConfigurationValue { get; set; }
    }
}
