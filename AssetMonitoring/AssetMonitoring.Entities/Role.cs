namespace AssetMonitoring.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AssetMonitoring.Components.Domain;

    public class Role : Entity
    {
        public Role()
        {
            this.Users = new HashSet<User>();
        }

        public override int Id { get; set ; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
