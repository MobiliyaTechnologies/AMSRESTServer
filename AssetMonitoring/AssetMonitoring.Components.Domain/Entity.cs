namespace AssetMonitoring.Components.Domain
{
    using System;

    public abstract class Entity
    {
        public abstract int Id { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
