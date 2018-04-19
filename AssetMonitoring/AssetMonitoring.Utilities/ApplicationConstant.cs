namespace AssetMonitoring.Utilities
{
    public static class ApplicationConstant
    {
        public static readonly string GatewayCapability = "Gateway";

        public static readonly string DeleteDocumentQueueName = "delete-sensor-document";
        public static readonly string DeleteRuleAlertQueueName = "delete-alert-document";
        public static readonly string UpdateDocumentGroupIdQueueName = "updated-sensor-group-id";
        public static readonly string CreateRuleJobQueueName = "create-sa-job";
        public static readonly string DeleteRuleJobQueueName = "delete-sa-job";
        public static readonly string DeleteGroupGpsQueueName = "delete-gps-document";

        public static readonly string PoisonQueueSuffix = "-poison";

        public static readonly string BlobPublicContainer = "asset-monitoring-public-container";

        public static readonly string DocumentDbDatabase = "AssetTrackingDB";

        public static readonly string DocumentDbGroupGpsDataCollection = "GroupGpsData";

        public static readonly string DocumentDbSensorDataCollection = "SensorData";

        public static readonly string DocumentDbAlertDataCollection = "AlertData";

        public static readonly int AlertCount = 1000;

        public static readonly string StreamAnalyticsAlertJobDbOutput = "AlertOutput";
        public static readonly string StreamAnalyticsAlertJobEventHubOutput = "EventHubOutput";
        public static readonly string StreamAnalyticsIotHubJobInput = "IotHubInput";
        public static readonly string StreamAnalyticsAlertJobNamePrefix = "GroupAlert_";
        public static readonly string StreamAnalyticsVibrationAlertJobNameSuffix = "_Vibration";
        public static readonly string StreamAnalyticsInvertAlertJobNameSuffix = "_Invert";

        public static readonly string GatewayRangeCapability = "GatewayRange";
        public static readonly string InvertCapability = "Invert";
        public static readonly string VibrationCapability = "Vibration";

        public static readonly string BulkDeleteStoreProcName = "BulkDelete";
        public static readonly string BulkImportStoreProcName = "BulkImport";

        public static readonly string DocumentDbDataPopulationJob = "Asset_DocumentDB_DataPopulation";
        public static readonly string DocumentDbSensorDataJobOutput = "SensorData";
        public static readonly string DocumentDbGpsDataJobOutput = "GroupGpsData";
    }
}
