namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;

    public class IndoorLayout
    {
        public IndoorLayout()
        {
            this.Gateways = new List<Gateway>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public string FileUrl { get; set; }

        public virtual List<Gateway> Gateways { get; set; }
    }
}
