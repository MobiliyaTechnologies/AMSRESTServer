using System.Linq;

namespace AssetMonitoring.StreamAnalytics.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.AnalyticsContract;
    using AssetMonitoring.Utilities;
    using Microsoft.Azure;
    using Microsoft.Azure.Management.StreamAnalytics;
    using Microsoft.Azure.Management.StreamAnalytics.Models;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public sealed class StreamAnalyticsService : IStreamAnalyticsService
    {
        private readonly string streamAnalyticsTransformationName = "TestTransformation";
        private readonly List<string> consumerGroups = new List<string>();
        private readonly string documentDBjobQuery =
                @"WITH input AS (select * from IotHubInput) 
                   
                    Select * into SensorData from input where GroupId >= 0 
                    
                    SELECT   
                        input.Latitude,  
                        input.Longitude,
                        input.Timestamp,
                        gids.ArrayValue  as groupid
                    into GroupGpsData
                    from input
                    CROSS APPLY GetArrayElements(input.GroupIds) AS gids
                    where GetArrayLength(input.GroupIds) != 0";

        public StreamAnalyticsService()
        {
            for (int i = 1; i <= ApplicationConfiguration.StreamAnalyticsAlertJobConsumerGroupCount; i++)
            {
                this.consumerGroups.Add(ApplicationConfiguration.StreamAnalyticsAlertJobConsumerGroupPrefix + i);
            }
        }

        async Task IStreamAnalyticsService.AddTransformation(JobTransformation jobTransformation)
        {
            var streamAnalyticsManagementClient = await this.GetStreamAnalyticsManagementClient();

            var transformationCreateParameters = new TransformationCreateOrUpdateParameters()
            {
                Transformation = new Transformation()
                {
                    Name = this.streamAnalyticsTransformationName,
                    Properties = new TransformationProperties()
                    {
                        Query = jobTransformation.Query,
                        StreamingUnits = ApplicationConfiguration.StreamAnalyticsStreamingUnits
                    }
                }
            };

            try
            {
                streamAnalyticsManagementClient.StreamingJobs.Stop(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobTransformation.JobName);
            }
            catch (Exception e)
            {
                // stop job throws exception if job is in invalid state, ignoring exception.
            }

            await streamAnalyticsManagementClient.Transformations.CreateOrUpdateAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobTransformation.JobName, transformationCreateParameters);

            // Start a Stream Analytics job
            JobStartParameters jobStartParameters = new JobStartParameters
            {
                OutputStartMode = OutputStartMode.JobStartTime,
            };

            await streamAnalyticsManagementClient.StreamingJobs.StartAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobTransformation.JobName, jobStartParameters);
        }

        async Task<string> IStreamAnalyticsService.CreateAlertJob(string jobName)
        {
            string currentAlertJobQuery = null;

            var streamAnalyticsManagementClient = await this.GetStreamAnalyticsManagementClient();

            var jobs = await streamAnalyticsManagementClient.StreamingJobs.ListJobsInResourceGroupAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, new JobListParameters("inputs,outputs,transformation"));

            var alertJob = jobs.Value.FirstOrDefault(j => j.Name.Equals(jobName));

            if (alertJob != null && alertJob.Properties.JobState.Equals("Running"))
            {
                if (alertJob.Properties.Transformation != null && alertJob.Properties.Transformation.Properties != null)
                {
                    currentAlertJobQuery = alertJob.Properties.Transformation.Properties.Query;
                }

                return currentAlertJobQuery;
            }

            if (alertJob == null)
            {
                await this.CreateJob(streamAnalyticsManagementClient, jobName);
            }

            var isSlopJob = jobName.EndsWith(ApplicationConstant.StreamAnalyticsVibrationAlertJobNameSuffix);

            if (alertJob == null || alertJob.Properties.JobState.Equals("Failed") || alertJob.Properties.Inputs.Count == 0)
            {
                var consumerGroup = this.GetConsumerGroup(jobs.Value.ToList(), isSlopJob);

                await this.CreateIotHubInput(streamAnalyticsManagementClient, jobName, consumerGroup);
            }

            if (alertJob == null || alertJob.Properties.JobState.Equals("Failed") || alertJob.Properties.Outputs.Count != 2)
            {
                // Create a Stream Analytics output target
                await this.CreateDoumentDbOutput(streamAnalyticsManagementClient, jobName, ApplicationConstant.DocumentDbAlertDataCollection, ApplicationConstant.StreamAnalyticsAlertJobDbOutput);

                OutputCreateOrUpdateParameters eventHubJobOutput = new OutputCreateOrUpdateParameters()
                {
                    Output = new Output()
                    {
                        Name = ApplicationConstant.StreamAnalyticsAlertJobEventHubOutput,
                        Properties = new OutputProperties()
                        {
                            Serialization = new JsonSerialization
                            {
                                Properties = new JsonSerializationProperties
                                {
                                    Encoding = "UTF8",
                                }
                            },
                            DataSource = new EventHubOutputDataSource()
                            {
                                Properties = new EventHubOutputDataSourceProperties()
                                {
                                    EventHubName = ApplicationConfiguration.EventHubName,
                                    ServiceBusNamespace = ApplicationConfiguration.EventHubNameSpace,
                                    SharedAccessPolicyName = ApplicationConfiguration.EventHubPolicyName,
                                    SharedAccessPolicyKey = ApplicationConfiguration.EventHubPolicyKey
                                }
                            }
                        }
                    }
                };

                await streamAnalyticsManagementClient.Outputs.CreateOrUpdateAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobName, eventHubJobOutput);
            }

            return currentAlertJobQuery;
        }

        async Task IStreamAnalyticsService.DeleteJob(string jobName)
        {
            var streamAnalyticsManagementClient = await this.GetStreamAnalyticsManagementClient();
            await streamAnalyticsManagementClient.StreamingJobs.DeleteAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobName);
        }

        async Task IStreamAnalyticsService.InitializeStreamAnalytics()
        {
            var jobName = ApplicationConstant.DocumentDbDataPopulationJob;
            var streamAnalyticsManagementClient = await this.GetStreamAnalyticsManagementClient();

            var jobs = await streamAnalyticsManagementClient.StreamingJobs.ListJobsInResourceGroupAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, new JobListParameters("inputs,outputs"));

            var documentDbJob = jobs.Value.FirstOrDefault(j => j.Name.Equals(jobName));

            if (documentDbJob != null && documentDbJob.Properties.JobState.Equals("Running"))
            {
                return;
            }

            if (documentDbJob == null)
            {
                await this.CreateJob(streamAnalyticsManagementClient, jobName);
            }

            if (documentDbJob == null || documentDbJob.Properties.Inputs.Count == 0)
            {
                await this.CreateIotHubInput(streamAnalyticsManagementClient, jobName);
            }

            if (documentDbJob == null || documentDbJob.Properties.Outputs.Count != 2)
            {
                await this.CreateDoumentDbOutput(streamAnalyticsManagementClient, jobName, ApplicationConstant.DocumentDbSensorDataCollection, ApplicationConstant.DocumentDbSensorDataJobOutput);

                await this.CreateDoumentDbOutput(streamAnalyticsManagementClient, jobName, ApplicationConstant.DocumentDbGroupGpsDataCollection, ApplicationConstant.DocumentDbGpsDataJobOutput);
            }

            var jobTransformation = new JobTransformation { JobName = jobName, Query = this.documentDBjobQuery };

            await (this as IStreamAnalyticsService).AddTransformation(jobTransformation);
        }

        private async Task<StreamAnalyticsManagementClient> GetStreamAnalyticsManagementClient()
        {
            var context = new AuthenticationContext(
               ApplicationConfiguration.ActiveDirectoryEndpoint +
                ApplicationConfiguration.ActiveDirectoryTenantId);

            var clientCredential = new ClientCredential(ApplicationConfiguration.ActiveDirectoryClientId, ApplicationConfiguration.ActiveDirectoryClientSecret);

            AuthenticationResult result = await context.AcquireTokenAsync(
               ApplicationConfiguration.StreamAnalyticsWindowsManagementUri
                , clientCredential);

            if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            {
                throw new InvalidOperationException("Failed to acquire token");
            }

            var aadTokenCredentials = new TokenCloudCredentials(
               ApplicationConfiguration.SubscriptionId,
                result.AccessToken);

            return new StreamAnalyticsManagementClient(aadTokenCredentials);
        }

        private string GetConsumerGroup(List<Job> jobs, bool isSlopJob)
        {
            var consumerGroupJobs = jobs.Where(v => (
            (v.Name.StartsWith(ApplicationConstant.StreamAnalyticsAlertJobNamePrefix) && !v.Name.EndsWith(ApplicationConstant.StreamAnalyticsVibrationAlertJobNameSuffix))
            || (v.Name.StartsWith(ApplicationConstant.StreamAnalyticsAlertJobNamePrefix) && v.Name.EndsWith(ApplicationConstant.StreamAnalyticsInvertAlertJobNameSuffix)))
            && v.Properties.Inputs.Any())
            .GroupBy(v => ((IoTHubStreamInputDataSource)((StreamInputProperties)v.Properties.Inputs.First().Properties).DataSource).Properties.ConsumerGroupName).ToDictionary(c => c.Key, c => c.Count());

            var consumerGroupSlopJobs = jobs.Where(v => v.Name.StartsWith(ApplicationConstant.StreamAnalyticsAlertJobNamePrefix) && v.Name.EndsWith(ApplicationConstant.StreamAnalyticsVibrationAlertJobNameSuffix) && v.Properties.Inputs.Any()).GroupBy(v => ((IoTHubStreamInputDataSource)((StreamInputProperties)v.Properties.Inputs.First().Properties).DataSource).Properties.ConsumerGroupName).ToDictionary(c => c.Key, c => c.Count());

            foreach (var consumerGroupSlopJob in consumerGroupSlopJobs)
            {
                if (consumerGroupJobs.ContainsKey(consumerGroupSlopJob.Key))
                {
                    consumerGroupJobs[consumerGroupSlopJob.Key] = consumerGroupJobs[consumerGroupSlopJob.Key] + 4;
                }
                else
                {
                    consumerGroupJobs.Add(consumerGroupSlopJob.Key, 4);
                }
            }

            var consumerGroupJob = isSlopJob ? consumerGroupJobs.FirstOrDefault(c => c.Value == 1) : consumerGroupJobs.FirstOrDefault(c => c.Value < 5);

            if (!consumerGroupJob.Equals(default(KeyValuePair<string, int>)))
            {
                return consumerGroupJob.Key;
            }

            if (consumerGroupJobs.Count == 19)
            {
                throw new InvalidOperationException("Maximum job limit reached");
            }

            return this.consumerGroups.First(c => !consumerGroupJobs.Any(g => g.Key.Equals(c)));
        }

        private async Task CreateDoumentDbOutput(StreamAnalyticsManagementClient streamAnalyticsManagementClient, string jobName, string collectionName, string outputName)
        {
            // Create a Stream Analytics output target
            OutputCreateOrUpdateParameters jobOutputCreateParameters = new OutputCreateOrUpdateParameters()
            {
                Output = new Output()
                {
                    Name = outputName,
                    Properties = new OutputProperties()
                    {
                        DataSource = new DocumentDbOutputDataSource()
                        {
                            Properties = new DocumentDbOutputDataSourceProperties()
                            {
                                AccountId = ApplicationConfiguration.DocumentDbAccountId,
                                AccountKey = ApplicationConfiguration.DocumentDbAuthKey,
                                CollectionNamePattern = collectionName,
                                Database = ApplicationConstant.DocumentDbDatabase,
                            }
                        }
                    }
                }
            };

            await streamAnalyticsManagementClient.Outputs.CreateOrUpdateAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobName, jobOutputCreateParameters);
        }

        private async Task CreateIotHubInput(StreamAnalyticsManagementClient streamAnalyticsManagementClient, string jobName, string consumerGroup = null)
        {
            var properties = new IoTHubStreamInputDataSourceProperties
            {
                SharedAccessPolicyKey = ApplicationConfiguration.IotHubSharedAccessPolicyKey,
                SharedAccessPolicyName = ApplicationConfiguration.IotHubSharedAccessPolicyName,
                IotHubNamespace = ApplicationConfiguration.IotHubHostName,
            };

            if (!string.IsNullOrWhiteSpace(consumerGroup))
            {
                properties.ConsumerGroupName = consumerGroup;
            }

            // Create a Stream Analytics input source
            InputCreateOrUpdateParameters jobInputCreateParameters = new InputCreateOrUpdateParameters()
            {
                Input = new Input()
                {
                    Name = ApplicationConstant.StreamAnalyticsIotHubJobInput,
                    Properties = new StreamInputProperties()
                    {
                        Serialization = new JsonSerialization
                        {
                            Properties = new JsonSerializationProperties
                            {
                                Encoding = "UTF8",
                            }
                        },
                        DataSource = new IoTHubStreamInputDataSource
                        {
                            Properties = properties
                        }
                    }
                }
            };

            await streamAnalyticsManagementClient.Inputs.CreateOrUpdateAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobName, jobInputCreateParameters);
        }

        private async Task CreateJob(StreamAnalyticsManagementClient streamAnalyticsManagementClient, string jobName)
        {
            JobCreateOrUpdateParameters jobCreateParameters = new JobCreateOrUpdateParameters()
            {
                Job = new Job()
                {
                    Name = jobName,
                    Location = ApplicationConfiguration.StreamAnalyticsJobLocation,
                    Properties = new JobProperties()
                    {
                        EventsOutOfOrderPolicy = EventsOutOfOrderPolicy.Adjust,
                        Sku = new Sku()
                        {
                            Name = "Standard"
                        }
                    }
                }
            };

            await streamAnalyticsManagementClient.StreamingJobs.CreateOrUpdateAsync(ApplicationConfiguration.StreamAnalyticsJobResourceGroup, jobCreateParameters);
        }
    }
}
