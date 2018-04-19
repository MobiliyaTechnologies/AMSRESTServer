namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System.Collections.Generic;

    public class GroupGpsDetail
    {
        public GroupGpsDetail()
        {
            this.Gps = new List<GpsDetail>();
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<GpsDetail> Gps { get; set; }
    }
}
