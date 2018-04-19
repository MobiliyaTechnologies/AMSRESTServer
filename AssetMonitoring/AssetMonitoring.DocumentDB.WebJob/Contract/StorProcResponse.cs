namespace AssetMonitoring.DocumentDB.WebJob.Contract
{
    public class StorProcResponse
    {
        public string deleted { get; set; }

        public bool continuation { get; set; }
    }
}
