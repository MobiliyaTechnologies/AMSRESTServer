namespace AssetMonitoring.DocumentDB.WebJob.Utilities
{
    using System;
    using System.Configuration;

    public static class ApplicationConfig
    {
        public static readonly string DBConnection = GetConfigData("DBConnection");
        public static readonly string EventHubConnectionString = GetConfigData("eventHub:ConnectionString");
        public static readonly string EventHubName = GetConfigData("eventHub:Name");
        public static readonly string ApplicationInsightsInstrumentationKey = GetConfigData("applicationInsights:InstrumentationKey");

        public static bool IsValidConfig()
        {
            return
                !(string.IsNullOrWhiteSpace(DBConnection) ||
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"] == null || string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString)
                || (ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"] == null
                || string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString)));
        }

        private static string GetConfigData(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
