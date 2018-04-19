namespace AssetMonitoring.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class Gateway : Entity
    {
        public override int Id { get; set; }

        [Required]
        public string GatewayKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string IotHubAccessTocken { get; set; }

        [ForeignKey("IndoorLayout")]
        public int? IndoorLayoutId { get; set; }

        public virtual IndoorLayout IndoorLayout { get; set; }

        public double? LayoutX { get; set; }

        public double? LayoutY { get; set; }
    }
}
