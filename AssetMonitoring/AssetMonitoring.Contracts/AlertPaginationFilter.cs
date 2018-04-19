namespace AssetMonitoring.Contracts
{
    public class AlertPaginationFilter
    {
        public int GroupId { get; set; }

        public string AssetBarcode { get; set; }

        public int? RuleId { get; set; }

        public int? PageSize { get; set; }

        public string ResponseContinuation { get; set; }
    }
}
