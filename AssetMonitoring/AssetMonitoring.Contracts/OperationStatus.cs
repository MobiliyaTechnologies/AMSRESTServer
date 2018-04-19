namespace AssetMonitoring.Contracts
{
    using AssetMonitoring.Contracts.Enums;

    public class OperationStatus
    {
        public OperationStatus(StatusCode statusCode = StatusCode.Ok, string message = "")
        {
            this.StatusCode = statusCode;
            this.Message = message;
        }

        public StatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}
