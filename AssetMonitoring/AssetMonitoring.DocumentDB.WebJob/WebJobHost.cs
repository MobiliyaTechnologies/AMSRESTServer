namespace AssetMonitoring.DocumentDB.WebJob
{
    using System;
    using System.Configuration;
    using System.Threading;
    using AssetMonitoring.DocumentDB.WebJob.Logger;
    using AssetMonitoring.DocumentDB.WebJob.Services;
    using AssetMonitoring.DocumentDB.WebJob.Utilities;
    using Microsoft.Azure.WebJobs;

    public class WebJobHost
    {
        public static void Main()
        {
            var jobActivator = new JobActivator();
            var applicationInsightsLogger = jobActivator.CreateInstance<ApplicationInsightsLogger>();

            try
            {
                while (!ApplicationConfig.IsValidConfig())
                {
                    Console.WriteLine("Invalid application configuration");

                    applicationInsightsLogger.LogException(new ConfigurationErrorsException("Invalid application configuration"));

                    Thread.Sleep(1000);
                }

                var config = new JobHostConfiguration { JobActivator = jobActivator };
                config.Queues.BatchSize = 1;

                var host = new JobHost(config);
                host.RunAndBlock();
            }
            catch (Exception ex)
            {
                applicationInsightsLogger.LogException(ex);

                throw;
            }
        }
    }
}
