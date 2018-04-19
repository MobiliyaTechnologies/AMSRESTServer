namespace AssetMonitoring.Contracts
{
    using AssetMonitoring.Contracts.Enums;

    public class User
    {
        public int Id { get; set; }

        public string B2cIdentifier { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public UserRole Role { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Authorization { get; set; }
    }
}
