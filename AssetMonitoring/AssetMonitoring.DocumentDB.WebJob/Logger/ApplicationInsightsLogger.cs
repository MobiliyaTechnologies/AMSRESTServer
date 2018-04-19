namespace AssetMonitoring.DocumentDB.WebJob.Logger
{
    using System;
    using System.Collections.Generic;
    using AssetMonitoring.DocumentDB.WebJob.Utilities;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Newtonsoft.Json;

    public sealed class ApplicationInsightsLogger
    {
        private readonly TelemetryClient telemetryClient;

        public ApplicationInsightsLogger()
        {
            this.telemetryClient = new TelemetryClient();
            this.telemetryClient.InstrumentationKey = ApplicationConfig.ApplicationInsightsInstrumentationKey;
        }

        public void LogException(Exception ex, Dictionary<string, string> logProperties = null)
        {
            try
            {
                var exceptionTelemetry = new ExceptionTelemetry(ex);

                if (logProperties != null)
                {
                    foreach (var logProperty in logProperties)
                    {
                        exceptionTelemetry.Properties.Add(logProperty.Key, logProperty.Value);
                    }
                }

                this.telemetryClient.TrackException(exceptionTelemetry);
                this.telemetryClient.Flush();
            }
            catch (Exception)
            {
                // ignore to avoid original exception overriding.
            }
        }

        public void LogException<T>(Exception ex, T queueMessage)
        {
            try
            {
                var exceptionTelemetry = new ExceptionTelemetry(ex);
                exceptionTelemetry.Properties.Add("QueueMessage", JsonConvert.SerializeObject(queueMessage));

                this.telemetryClient.TrackException(exceptionTelemetry);
                this.telemetryClient.Flush();
            }
            catch (Exception)
            {
                // ignore to avoid original exception overriding.
            }
        }
    }
}
