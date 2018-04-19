using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetMonitoring.Contracts
{
    public class Gateway
    {
        public Gateway()
        {
            this.GatewaytAsset = new List<GatewaytAsset>();
        }

        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed in between words.")]
        public string GatewayKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public double? LayoutX { get; set; }

        public double? LayoutY { get; set; }

        public List<GatewaytAsset> GatewaytAsset { get; set; }
    }
}
