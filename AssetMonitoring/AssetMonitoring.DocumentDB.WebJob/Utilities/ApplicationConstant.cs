namespace AssetMonitoring.DocumentDB.WebJob.Utilities
{
    public static class ApplicationConstant
    {
        public static readonly string BulkDeleteStoreProcName = "BulkDelete";

        public static readonly string BulkImportStoreProcName = "BulkImport";

        public static readonly string DocumentDbDatabase = "AssetTrackingDB";

        public static readonly string DocumentDbSensorDataCollection = "SensorData";

        public static readonly string DocumentDbSensorDataPartitionKey = "/GroupId";

        public static readonly string DocumentDbGroupGpsDataCollection = "GroupGpsData";

        public static readonly string DocumentDbAlertDataCollection = "AlertData";

        public static readonly string DocumentDbAlertDataPartitionKey = "/groupid";
    }
}
