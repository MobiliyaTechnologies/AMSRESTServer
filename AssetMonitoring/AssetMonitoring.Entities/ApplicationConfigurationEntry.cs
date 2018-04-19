namespace AssetMonitoring.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class ApplicationConfigurationEntry : Entity
    {
        public override int Id { get; set; }

        public string ConfigurationKey { get; set; }

        public string ConfigurationValue { get; set; }

        [ForeignKey("ApplicationConfiguration")]
        public int? ApplicationConfigurationId { get; set; }

        public virtual ApplicationConfiguration ApplicationConfiguration { get; set; }
    }
}
