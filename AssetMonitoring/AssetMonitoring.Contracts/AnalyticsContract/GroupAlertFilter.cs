namespace AssetMonitoring.Contracts.AnalyticsContract
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GroupAlertFilter
    {
        public GroupAlertFilter()
        {
            this.GroupRules = new List<GroupRule>();
        }

        [Required]
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<GroupRule> GroupRules { get; set; }
    }
}
