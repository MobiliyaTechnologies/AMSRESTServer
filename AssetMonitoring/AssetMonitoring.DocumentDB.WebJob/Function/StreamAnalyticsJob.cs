namespace AssetMonitoring.DocumentDB.WebJob.Function
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.AnalyticsContract;
    using AssetMonitoring.DocumentDB.WebJob.Logger;
    using AssetMonitoring.DocumentDB.WebJob.Utilities;
    using AssetMonitoring.StreamAnalytics.Services;
    using Microsoft.Azure.WebJobs;

    public class StreamAnalyticsJob
    {
        private readonly ApplicationInsightsLogger applicationInsightsLogger;
        private readonly IGroupAlertService groupAlertService;
        private SqlConnection sqlConnection;
        private SqlTransaction sqlTransaction;

        public StreamAnalyticsJob(IGroupAlertService groupAlertService, ApplicationInsightsLogger applicationInsightsLogger)
        {
            this.groupAlertService = groupAlertService;
            this.applicationInsightsLogger = applicationInsightsLogger;
        }

        public async Task CreateSAJob([QueueTrigger("create-sa-job")] int groupId, TextWriter log)
        {
            try
            {
                using (this.sqlConnection = new SqlConnection(ApplicationConfig.DBConnection))
                {
                    this.sqlConnection.Open();

                    using (this.sqlTransaction = this.sqlConnection.BeginTransaction())
                    {
                        var groupAlertFilter = this.GetAlert(groupId);

                        if (groupAlertFilter.GroupRules.Count > 0)
                        {
                            Console.WriteLine("Rules job creation started for Group Id - {0}", groupId);

                            await this.groupAlertService.ApplyGroupRule(groupAlertFilter);

                            var ruleCreationMessage = string.Format("Sensor rule updated for Sensor Group {0}.", groupAlertFilter.GroupName);

                            EventHubHelper.SendMessage(new { RuleCreationMessage = ruleCreationMessage });

                            Console.WriteLine("Rules job created for Group Id - {0}", groupId);
                        }
                        else
                        {
                            await this.DeleteSAJob(groupId);
                        }

                        this.UpdateGroup(groupId);

                        this.sqlTransaction.Commit();
                    }

                    this.sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                this.applicationInsightsLogger.LogException(ex, groupId);

                throw;
            }
        }

        public async Task DeleteSAJob([QueueTrigger("delete-sa-job")] int groupId, TextWriter log)
        {
            try
            {
                await this.DeleteSAJob(groupId);
            }
            catch (Exception ex)
            {
                this.applicationInsightsLogger.LogException(ex, groupId);

                throw;
            }
        }

        private GroupAlertFilter GetAlert(int groupId)
        {
            var alertQuery = @"SELECT SR.[Id]
                                      , SCF.[CapabilityId]
	                                  ,	SCF.[Name]
	                                  ,	SCF.[Operator]
                                      , [MinThreshold]     
                                      , [MaxThreshold]
                                      , SG.[Name] as GroupName
                                FROM[dbo].[SensorRules] SR
                                inner join[dbo].[SensorCapabilityFilters] SCF on SR.CapabilityFilterId = SCF.Id
                                inner join[dbo].[SensorGroups] SG on SR.SensorGroupId = SG.Id
                                where SR.SensorGroupId = @SensorGroupId";

            var groupAlertFilter = new GroupAlertFilter { GroupId = groupId };

            using (SqlCommand sqlCommand = new SqlCommand(alertQuery, this.sqlConnection, this.sqlTransaction))
            {
                sqlCommand.Parameters.AddWithValue("@SensorGroupId", groupId);

                using (SqlDataReader result = sqlCommand.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var groupRule = new GroupRule
                        {
                            RuleId = SqlTypeConverter.ToInt32(result["Id"]),
                            CapabilityId = SqlTypeConverter.ToInt32(result["CapabilityId"]),
                            Filter = SqlTypeConverter.ToString(result["Name"]),
                            MaxThreshold = SqlTypeConverter.ToString(result["MaxThreshold"]),
                            MinThreshold = SqlTypeConverter.ToString(result["MinThreshold"]),
                            Operator = SqlTypeConverter.ToString(result["Operator"])
                        };

                        groupAlertFilter.GroupName = SqlTypeConverter.ToString(result["GroupName"]);
                        groupAlertFilter.GroupRules.Add(groupRule);
                    }

                    result.Close();
                }
            }

            return groupAlertFilter;
        }

        private void UpdateGroup(int groupId)
        {
            var query = "UPDATE [dbo].[SensorGroups] set RuleCreationInProgress = @RuleCreationInProgress  where [Id] = @Id";

            using (SqlCommand sqlCommand = new SqlCommand(query, this.sqlConnection, this.sqlTransaction))
            {
                sqlCommand.Parameters.AddWithValue("@RuleCreationInProgress", 0);
                sqlCommand.Parameters.AddWithValue("@Id", groupId);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private async Task DeleteSAJob(int groupId)
        {
            Console.WriteLine("Rules job deletion started for Group Id - {0}", groupId);

            await this.groupAlertService.DeleteGroup(groupId);

            Console.WriteLine("Rules job deletion completed for Group Id - {0}", groupId);
        }
    }
}
