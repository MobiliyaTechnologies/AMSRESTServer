namespace AssetMonitoring.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using AssetMonitoring.Components.Domain;

    public class User : Entity
    {
        public override int Id { get; set; }

        public string B2cIdentifier { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        [ForeignKey("Role")]
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
