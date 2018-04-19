namespace AssetMonitoring.Contracts
{
    using System.ComponentModel.DataAnnotations;

    public class Sensor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string SensorKey { get; set; }

        [Required]
        public int SensorTypeId { get; set; }

        public int SensorGroupId { get; set; }

        public string SensorGroupName { get; set; }
    }
}