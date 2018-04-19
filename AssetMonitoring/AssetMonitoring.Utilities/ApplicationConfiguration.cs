namespace AssetMonitoring.Utilities
{
    using System;
    using System.Configuration;

    public static class ApplicationConfiguration
    {
        public static readonly string IotHubConnectionString = GetConfigData("iotHub:ConnectionString");
        public static readonly string IotHubHostName = GetConfigData("iotHub:HostName");
        public static readonly string IotHubSharedAccessPolicyKey = GetConfigData("iotHub:SharedAccessPolicyKey");
        public static readonly string IotHubSharedAccessPolicyName = GetConfigData("iotHub:SharedAccessPolicyName");

        public static readonly string B2cAadInstance = GetConfigData("b2c:AadInstance");
        public static readonly string B2cTenant = GetConfigData("b2c:Tenant");
        public static readonly string B2cClientId = GetConfigData("b2c:ClientId");

        public static readonly string B2cSignUpInPolicyId = GetConfigData("b2c:SignUpInPolicyId");

        public static readonly string B2cMobileRedirectUrl = GetConfigData("b2c:MobileRedirectUrl");
        public static readonly string B2cMobileInstanceUrl = GetConfigData("b2c:MobileInstanceUrl");
        public static readonly string B2cMobileTokenUrl = GetConfigData("b2c:MobileTokenUrl");

        public static readonly string BlobStorageConnectionString = GetConfigData("blobStorage:ConnectionString");

        public static readonly string DocumentDbEndpoint = GetConfigData("documentDB:Endpoint");
        public static readonly string DocumentDbAuthKey = GetConfigData("documentDB:AuthKey");
        public static readonly string DocumentDbAccountId = GetConfigData("documentDB:AccountId");
        public static readonly int DocumentDbOfferThroughput = Convert.ToInt32(GetConfigData("documentDB:OfferThroughput"));

        public static readonly string SimulatorDbServer = GetConfigData("simulator:DbServer");
        public static readonly string SimulatorDbName = GetConfigData("simulator:DbName");
        public static readonly string SimulatorDbUserId = GetConfigData("simulator:DbUserId");
        public static readonly string SimulatorDbPassword = GetConfigData("simulator:DbPassword");

        public static readonly string StreamAnalyticsWindowsManagementUri = GetConfigData("sa:WindowsManagementUri");
        public static readonly string StreamAnalyticsReportJobName = GetConfigData("sa:ReportJobName");
        public static readonly string StreamAnalyticsJobResourceGroup = GetConfigData("sa:JobResourceGroup");
        public static readonly string StreamAnalyticsJobLocation = GetConfigData("sa:JobLocation");
        public static readonly string StreamAnalyticsAlertJobTAble = GetConfigData("sa:AlertJobTAble");
        public static readonly int StreamAnalyticsStreamingUnits = Convert.ToInt32(GetConfigData("sa:StreamingUnits"));

        public static readonly string StreamAnalyticsAlertJobConsumerGroupPrefix = GetConfigData("sa:AlertJobConsumerGroupPrefix");
        public static readonly int StreamAnalyticsAlertJobConsumerGroupCount = Convert.ToInt32(GetConfigData("sa:AlertJobConsumerGroupCount"));

        public static readonly string EventHubName = GetConfigData("eventHub:Name");
        public static readonly string EventHubPolicyName = GetConfigData("eventHub:PolicyName");
        public static readonly string EventHubPolicyKey = GetConfigData("eventHub:PolicyKey");
        public static readonly string EventHubNameSpace = GetConfigData("eventHub:NameSpace");

        public static readonly string ActiveDirectoryEndpoint = GetConfigData("ad:Endpoint");
        public static readonly string SubscriptionId = GetConfigData("ad:SubscriptionId");
        public static readonly string ActiveDirectoryTenantId = GetConfigData("ad:TenantId");
        public static readonly string ActiveDirectoryClientId = GetConfigData("ad:ClientId");
        public static readonly string ActiveDirectoryClientSecret = GetConfigData("ad:ClientSecret");

        public static readonly string ApplicationInsightsInstrumentationKey = GetConfigData("applicationInsights:InstrumentationKey");

        private static string GetConfigData(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
