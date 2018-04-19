namespace AssetMonitoring.Contracts
{
    using System.Collections.Generic;
    using AssetMonitoring.Contracts.DocumentDbContract;

    public class AlertPaginationResult
    {
        public AlertPaginationResult()
        {
            this.Result = new List<Alert>();
        }

        public int Count { get; set; }

        public string ResponseContinuation { get; set; }

        public List<Alert> Result { get; set; }
    }
}
