namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;

    public class GroupAlertPaginateFilter
    {
        public GroupAlertPaginateFilter()
        {
            this.AssetBarcodes = new List<string>();
            this.GroupRules = new List<GroupRuleAlertFilter>();
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<string> AssetBarcodes { get; set; }

        public List<GroupRuleAlertFilter> GroupRules { get; set; }

    }
}
