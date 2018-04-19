namespace AssetMonitoring.Contracts
{
    using System.ComponentModel.DataAnnotations;

    public class CapabilityFilter
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string MinValue { get; set; }

        public string MaxValue { get; set; }

        public string Operator { get; set; }

        public int CapabilityId { get; set; }
    }
}