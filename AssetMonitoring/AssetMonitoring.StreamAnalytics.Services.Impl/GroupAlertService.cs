namespace AssetMonitoring.StreamAnalytics.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.AnalyticsContract;
    using AssetMonitoring.Utilities;

    public sealed class GroupAlertService : IGroupAlertService
    {
        private readonly IStreamAnalyticsService streamAnalyticsService;

        public GroupAlertService(IStreamAnalyticsService streamAnalyticsService)
        {
            this.streamAnalyticsService = streamAnalyticsService;
        }

        async Task IGroupAlertService.ApplyGroupRule(GroupAlertFilter groupAlertFilter)
        {
            var transformationTask = new List<Task>();

            var jobquery = @"With BreakRule as (select 
                case 
                {0} 
                Else null 
                End As [Value],
                case  
                {1}
                Else null
                End As [SensorRuleId], 
                SensorKey , AssetBarcode , Timestamp, GroupId  from IotHubInput
                where GroupId = " + groupAlertFilter.GroupId +
                @" and ( {2} ) )  

                select *  into " + ApplicationConstant.StreamAnalyticsAlertJobDbOutput +
                @" from  BreakRule 
                
                select AssetBarcode, SensorRuleId  into " + ApplicationConstant.StreamAnalyticsAlertJobEventHubOutput + " from BreakRule ";

            var valueCase = string.Empty;
            var ruleCase = string.Empty;
            var condition = string.Empty;
            var jobName = ApplicationConstant.StreamAnalyticsAlertJobNamePrefix + groupAlertFilter.GroupId;

            var rules = groupAlertFilter.GroupRules.Where(r => !string.IsNullOrWhiteSpace(r.Operator));

            foreach (var rule in rules)
            {
                switch (rule.Operator)
                {
                    case "range":
                        valueCase = valueCase + string.Format(" when ({0} < {1} or {0} > {2}) then {0} ", rule.Filter, rule.MinThreshold, rule.MaxThreshold);

                        ruleCase = ruleCase + string.Format(" when ({0} < {1} or {0} > {2}) then {3} ", rule.Filter, rule.MinThreshold, rule.MaxThreshold, rule.RuleId);

                        if (!string.IsNullOrEmpty(condition))
                        {
                            condition = condition + " or ";
                        }

                        condition = condition + string.Format(" ({0} < {1} or {0} > {2}) ", rule.Filter, rule.MinThreshold, rule.MaxThreshold);
                        break;

                    case "equal":
                        valueCase = valueCase + string.Format(" when ({0} != {1}) then {0} ", rule.Filter, rule.MinThreshold);

                        ruleCase = ruleCase + string.Format(" when ({0} != {1}) then {2} ", rule.Filter, rule.MinThreshold, rule.RuleId);

                        if (!string.IsNullOrEmpty(condition))
                        {
                            condition = condition + " or ";
                        }

                        condition = condition + string.Format(" ({0} != {1}) ", rule.Filter, rule.MinThreshold);
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Operator {0} not mapped.", rule.Operator));
                }
            }

            var gatewayRangeRule = groupAlertFilter.GroupRules.FirstOrDefault(r => r.Filter.Equals(ApplicationConstant.GatewayRangeCapability));

            if (gatewayRangeRule != null)
            {
                var gatewayRange = -33;

                if (gatewayRangeRule.MaxThreshold == "Far")
                {
                    gatewayRange = -66;
                }
                else if (gatewayRangeRule.MaxThreshold == "TooFar")
                {
                    gatewayRange = -100;
                }

                valueCase = valueCase + string.Format(" when ( IoTHub.ConnectionDeviceId = '{0}' and RSSI < {1}) then RSSI ", gatewayRangeRule.MinThreshold, gatewayRange);

                ruleCase = ruleCase + string.Format(" when ( IoTHub.ConnectionDeviceId = '{0}' and RSSI < {1}) then {2} ", gatewayRangeRule.MinThreshold, gatewayRange, gatewayRangeRule.RuleId);

                if (!string.IsNullOrEmpty(condition))
                {
                    condition = condition + " or ";
                }

                condition = condition + string.Format(" ( IoTHub.ConnectionDeviceId = '{0}' and RSSI < {1}) ", gatewayRangeRule.MinThreshold, gatewayRange);
            }

            var gatewayRule = groupAlertFilter.GroupRules.FirstOrDefault(r => r.Filter.Equals(ApplicationConstant.GatewayCapability));

            if (gatewayRule != null)
            {
                valueCase = valueCase + string.Format(" when ( IoTHub.ConnectionDeviceId != '{0}' ) then IoTHub.ConnectionDeviceId ", gatewayRule.MinThreshold);

                ruleCase = ruleCase + string.Format(" when ( IoTHub.ConnectionDeviceId != '{0}' ) then {1} ", gatewayRule.MinThreshold, gatewayRule.RuleId);

                if (!string.IsNullOrEmpty(condition))
                {
                    condition = condition + " or ";
                }

                condition = condition + string.Format(" (IoTHub.ConnectionDeviceId != '{0}') ", gatewayRule.MinThreshold);
            }

            if (!string.IsNullOrWhiteSpace(valueCase) && !string.IsNullOrWhiteSpace(ruleCase) && !string.IsNullOrWhiteSpace(condition))
            {
                var currentJobQuery = await this.streamAnalyticsService.CreateAlertJob(jobName);

                jobquery = string.Format(jobquery, valueCase, ruleCase, condition);

                if (!jobquery.Equals(currentJobQuery))
                {
                    var jobTransformation = new JobTransformation { JobName = jobName, Query = jobquery };
                    transformationTask.Add(this.streamAnalyticsService.AddTransformation(jobTransformation));
                }
            }
            else
            {
                // delete group alert job if exists
                transformationTask.Add(this.streamAnalyticsService.DeleteJob(jobName));
            }

            await this.ApplyVibrationRule(groupAlertFilter, transformationTask);
            await this.ApplyInvertRule(groupAlertFilter, transformationTask);

            await Task.WhenAll(transformationTask);
        }

        async Task IGroupAlertService.DeleteGroup(int groupId)
        {
            var transformationTask = new List<Task>();

            // delete alert job
            var jobName = ApplicationConstant.StreamAnalyticsAlertJobNamePrefix + groupId;
            transformationTask.Add(this.streamAnalyticsService.DeleteJob(jobName));

            // delete slop job
            jobName = ApplicationConstant.StreamAnalyticsAlertJobNamePrefix + groupId + ApplicationConstant.StreamAnalyticsVibrationAlertJobNameSuffix;
            transformationTask.Add(this.streamAnalyticsService.DeleteJob(jobName));

            // delete invert job
            jobName = ApplicationConstant.StreamAnalyticsAlertJobNamePrefix + groupId + ApplicationConstant.StreamAnalyticsInvertAlertJobNameSuffix;
            transformationTask.Add(this.streamAnalyticsService.DeleteJob(jobName));

            await Task.WhenAll(transformationTask);
        }

        private async Task ApplyVibrationRule(GroupAlertFilter groupAlertFilter, List<Task> transformationTask)
        {
            var capabilities = groupAlertFilter.GroupRules.Where(r => r.Filter.Equals(ApplicationConstant.VibrationCapability));

            var vibrationJobName = ApplicationConstant.StreamAnalyticsAlertJobNamePrefix + groupAlertFilter.GroupId + ApplicationConstant.StreamAnalyticsVibrationAlertJobNameSuffix;

            if (capabilities.Count() == 0)
            {
                // delete vibration job if exists
                transformationTask.Add(this.streamAnalyticsService.DeleteJob(vibrationJobName));
                return;
            }

            var capabilityFilter = string.Empty;
            var ruleCase = string.Empty;

            foreach (var capability in capabilities)
            {
                if (capabilityFilter != string.Empty)
                {
                    capabilityFilter = capabilityFilter + " OR ";
                }

                capabilityFilter = capabilityFilter + string.Format("CapabilityId = {0}", capability.CapabilityId);

                ruleCase = ruleCase + string.Format(" WHEN {0} THEN {1} ", capability.CapabilityId, capability.RuleId);
            }

            var vibrationQuery = @"WITH entity AS (SELECT * FROM IOTHubInput TIMESTAMP BY [TimeStamp] 
                WHERE  GroupId = " + groupAlertFilter.GroupId + " and (" + capabilityFilter +
                @" ) )

                , FirstInstance AS(
                    SELECT MAX(x) AS [FirstX]
                    , MAX(y) AS [FirstY]
                    , MAX(z) AS [FirstZ]
                    , MAX([TimeStamp]) AS [TimeStamp]
                    , [SensorKey]
                    , [GroupId]
                    , [AssetBarCode]
                    , [CapabilityId]
                    FROM entity
                    GROUP BY
                    HOPPINGWINDOW(DURATION(ss,5), HOP(ss, 1))
	                ,[GroupId]
                    ,[SensorKey]
                    ,[AssetBarCode]
                    ,[CapabilityId]
                )

                , SecondInstance AS(
                    SELECT MAX(x) AS [LastX]
                    , MAX(y) AS [LastY]
                    , MAX(z) AS [LastZ]
                       , MAX([TimeStamp]) AS [TimeStamp]
                    , [SensorKey]
                    FROM entity
                    GROUP BY TUMBLINGWINDOW(ss,2)
	                ,[SensorKey]
                    ,[AssetBarCode]
                    ,[CapabilityId]
                )

                , Slope1 AS(
                SELECT FI.[SensorKey]
                , FI.[AssetBarCode]
                , FI.[CapabilityId]
                , FI.[GroupId]
                , MAX(((ABS(SI.[LastX] - FI.[FirstX])- (-30))/(50 - (-30)))*(50 - (-30))) AS[SlopeOfX]
                ,MAX(((ABS(SI.[LastY] - FI.[FirstY])- (-30))/(50 - (-30)))*(50 - (-30))) AS[SlopeOfY]
                ,MAX(((ABS(SI.[LastZ] - FI.[FirstZ])- (-30))/(50 - (-30)))*(50 - (-30))) AS[SlopeOfZ]
                ,MAX(FI.[TimeStamp]) AS[TimeStamp]
                FROM[FirstInstance] FI
                INNER JOIN[SecondInstance] SI
                ON FI.[SensorKey] = SI.[SensorKey]
                AND DATEDIFF(ss, FI, SI) = 1
                GROUP BY HOPPINGWINDOW(DURATION(ss,5), HOP(ss,1))
                    ,FI.[GroupId]
	                ,FI.[SensorKey]
                    ,FI.[AssetBarCode]
                    ,FI.[CapabilityId]
                )

                , Slope2 AS(
                SELECT FI.[SensorKey]
                , MAX(((ABS(SI.[LastX] - FI.[FirstX])- (-30))/(50 - (-30)))*(50 - (-30))) AS[SlopeOfX]
                ,MAX(((ABS(SI.[LastY] - FI.[FirstY])- (-30))/(50 - (-30)))*(50 - (-30))) AS[SlopeOfY]
                ,MAX(((ABS(SI.[LastZ] - FI.[FirstZ])- (-30))/(50 - (-30)))*(50 - (-30))) AS[SlopeOfZ]
                ,MAX(FI.[TimeStamp]) AS[TimeStamp]
                FROM[FirstInstance] FI
                JOIN[SecondInstance] SI
                ON FI.[SensorKey] = SI.[SensorKey]
                AND DATEDIFF(ss, FI, SI) = 1
                GROUP BY TUMBLINGWINDOW(ss,2)
	                ,FI.[SensorKey]
                    ,FI.[AssetBarCode]
                    ,FI.[CapabilityId]
                )

                , Compare AS(
                SELECT S1.[SensorKey]
                , S1.[AssetBarCode]
                , S1.[CapabilityId]
                , S1.[GroupId]
                , MAX(S2.[SlopeOfX] - S1.[SlopeOfX]) AS[ValueX]
                ,MAX(S2.[SlopeOfY] - S1.[SlopeOfY]) AS[ValueY]
                ,MAX(S2.[SlopeOfZ] - S1.[SlopeOfZ]) AS[ValueZ]
                ,MAX(S1.[Timestamp]) AS[TimeStamp]
                FROM Slope1 S1
                JOIN Slope2 S2
                ON S1.[Sensorkey] = S2.[SensorKey]
                AND DATEDIFF(ss, S1, S2) = 1
                GROUP BY TUMBLINGWINDOW(ss,2)
                ,S1.[GroupId]
                ,S1.[SensorKey]
                ,S1.[AssetBarCode]
                ,S1.[CapabilityId]
                )

                , Query AS(
                SELECT
                CO.[SensorKey]
                , CO.[AssetBarCode]
                , CO.[GroupId]
                , CONCAT('Abnormal shake observed in ', CO.[AssetBarCode])  AS[Value]
                ,MAX(CO.[TimeStamp]) AS[TimeStamp]
                ,CASE CO.[CapabilityId] " + ruleCase +
                  @"  Else null 
                    END AS [SensorRuleId]
                FROM Compare CO
                WHERE ABS(CO.[ValueX]) >= 10 OR ABS(CO.[ValueY]) >= 10 OR ABS(CO.[ValueZ]) >= 10
                GROUP BY TUMBLINGWINDOW(ss,2)
                , CO.[GroupId]
                ,CO.[SensorKey]
                ,CO.[AssetBarCode]
                ,CO.[CapabilityId]
                )

                SELECT[SensorKey]
                ,[Value]
                ,[AssetBarCode]
                ,[TimeStamp]
                ,[SensorRuleId]
                ,[GroupId]
                        INTO " + ApplicationConstant.StreamAnalyticsAlertJobDbOutput +
                @" FROM Query 

                SELECT
                [AssetBarCode],[SensorRuleId]            
                INTO " + ApplicationConstant.StreamAnalyticsAlertJobEventHubOutput + " FROM Query";

            var currentJobQuery = await this.streamAnalyticsService.CreateAlertJob(vibrationJobName);

            if (!vibrationQuery.Equals(currentJobQuery))
            {
                var jobTransformation = new JobTransformation { JobName = vibrationJobName, Query = vibrationQuery };
                transformationTask.Add(this.streamAnalyticsService.AddTransformation(jobTransformation));
            }
        }

        private async Task ApplyInvertRule(GroupAlertFilter groupAlertFilter, List<Task> transformationTask)
        {
            var invertRule = groupAlertFilter.GroupRules.Where(r => r.Filter.Equals(ApplicationConstant.InvertCapability)).FirstOrDefault();

            var invertJobName = ApplicationConstant.StreamAnalyticsAlertJobNamePrefix + groupAlertFilter.GroupId + ApplicationConstant.StreamAnalyticsInvertAlertJobNameSuffix;

            if (invertRule == null)
            {
                // delete invert job if exists
                transformationTask.Add(this.streamAnalyticsService.DeleteJob(invertJobName));
                return;
            }

            var invertQuery = @"WITH entity AS 
                            (
                                SELECT *
                                FROM
                                IotHubInput TIMESTAMP BY [Timestamp]
                                WHERE   GroupId = {0} and CapabilityId = {1})
                            ,Store AS
                            (
                                SELECT SUM(x) AS [x]
                                    ,SUM(y) AS [y]
                                    ,SUM(z) AS [z]
                                    ,[SensorKey]
                                    ,[AssetBarCode]
                                    ,[GroupId]
                                    ,MAX([TimeStamp]) AS [TimeStamp]
                                    ,{2} AS [SensorRuleId]
                                FROM entity
                                    GROUP BY TUMBLINGWINDOW(ss,5)
                                        ,[GroupId]
                                        ,[SensorKey]
                                        ,[AssetBarCode]
                                        ,[SensorRuleId]
                            ), BreakRule As
                           ( SELECT 
                                [SensorKey]
                                ,CASE
                                    WHEN
                                        (ST.[x] > 0 
                                        AND ST.[y] > 0
                                        AND ST.[z] > 0)
                                    THEN 'Package is inverted'
                                    WHEN 
                                        (ST.[x] < 0 
                                        AND ST.[y] < 0 
                                        AND ST.[z] > 0)
                                    THEN 'Package is not in upright position'
                                    ELSE NULL
                                END AS [Value]
                                ,[AssetBarCode]
                                ,[TimeStamp]
                                ,[SensorRuleId]
                                ,[GroupId]
                            FROM Store ST
                                WHERE 
                                    (ST.[x] > 0 
                                    AND ST.[y] > 0
                                    AND ST.[z] > 0)
                                    OR
                                    (ST.[x] < 0 
                                    AND ST.[y] < 0 
                                    AND ST.[z] > 0)
                            )
                           
                            select *  into " + ApplicationConstant.StreamAnalyticsAlertJobDbOutput +
                            @" from  BreakRule 
                             select AssetBarcode, SensorRuleId  into " + ApplicationConstant.StreamAnalyticsAlertJobEventHubOutput + " from BreakRule ";

            var currentJobQuery = await this.streamAnalyticsService.CreateAlertJob(invertJobName);

            invertQuery = string.Format(invertQuery, groupAlertFilter.GroupId, invertRule.CapabilityId, invertRule.RuleId);

            if (!invertQuery.Equals(currentJobQuery))
            {
                var jobTransformation = new JobTransformation { JobName = invertJobName, Query = invertQuery };
                transformationTask.Add(this.streamAnalyticsService.AddTransformation(jobTransformation));
            }
        }
    }
}
