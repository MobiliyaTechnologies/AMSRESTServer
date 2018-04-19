namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System.Collections.Generic;

    public class DeleteRuleAlertDetail
    {
        public DeleteRuleAlertDetail()
        {
            this.SensorRuleIds = new List<int>();
        }

        public int GroupId { get; set; }

        public List<int> SensorRuleIds { get; set; }
    }
}
