namespace AssetMonitoring.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services.Impl.Mappings;
    using Contracts.AnalyticsContract;

    public sealed class SensorRuleService : ISensorRuleService
    {
        private readonly IRepository repository;
        private readonly IGroupAlertService groupAlertService;
        private readonly IAlertService alertService;

        public SensorRuleService(IRepository repository, IGroupAlertService groupAlertService, IAlertService alertService)
        {
            this.repository = repository;
            this.groupAlertService = groupAlertService;
            this.alertService = alertService;
        }

        async Task<OperationStatus> ISensorRuleService.Create(int sensorGroupId, List<SensorRule> sensorRules)
        {
            var group = this.repository.Read<Entities.SensorGroup>(sensorGroupId);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", sensorGroupId));
            }

            if (group.RuleCreationInProgress)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor rule creation is in progress for group - {0}, can not create new rule. Please try after some time.", group.Name));
            }

            var groupCapabilityFilterIds = group.Sensors.SelectMany(s => s.SensorType.Capabilities).SelectMany(c => c.SensorCapabilityFilters).Select(f => f.Id).Distinct();

            foreach (var sensorRule in sensorRules)
            {
                var filter = this.repository.Read<Entities.SensorCapabilityFilter>(sensorRule.CapabilityFilterId);

                if (filter == null)
                {
                    return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Capability filter does not exist for id - {0}.", sensorRule.CapabilityFilterId));
                }

                if (!groupCapabilityFilterIds.Any(id => id == sensorRule.CapabilityFilterId))
                {
                    return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Sensor group does not support Capability filter id - {0}.", sensorRule.CapabilityFilterId));
                }

                var rule = this.repository.Query<Entities.SensorRule>().FirstOrDefault(r => r.SensorGroupId == sensorGroupId && r.CapabilityFilterId == sensorRule.CapabilityFilterId);

                if (rule != null)
                {
                    rule.MinThreshold = sensorRule.MinThreshold;
                    rule.MaxThreshold = sensorRule.MaxThreshold;
                }
                else
                {
                    rule = new Entities.SensorRule
                    {
                        MinThreshold = sensorRule.MinThreshold,
                        MaxThreshold = sensorRule.MaxThreshold,
                        SensorGroupId = sensorGroupId,
                        CapabilityFilterId = sensorRule.CapabilityFilterId
                    };
                }

                this.repository.Persist(rule);
            }



            if (sensorRules.Count > 0)
            {
                group.RuleCreationInProgress = true;
                this.repository.Persist(group);

                await this.groupAlertService.ApplyGroupFilter(sensorGroupId);
            }

            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorRuleService.Delete(int sensorRuleId)
        {
            var rule = this.repository.Read<Entities.SensorRule>(sensorRuleId);

            if (rule == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor rule does not exist for id - {0}.", sensorRuleId));
            }

            var sensorGroup = rule.SensorGroup;

            if (sensorGroup.RuleCreationInProgress)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor rule creation is in progress for group - {0}, can not delete rule. Please try after some time.", sensorGroup.Name));
            }

            this.repository.Delete(rule);

            await this.alertService.DeleteAlerts(sensorGroup.Id, new List<int> { sensorRuleId });

            sensorGroup.RuleCreationInProgress = true;
            this.repository.Persist(sensorGroup);

            await this.groupAlertService.ApplyGroupFilter(sensorGroup.Id);

            return new OperationStatus();
        }

        SensorRule ISensorRuleService.Get(int sensorRuleId)
        {
            var rule = this.repository.Read<Entities.SensorRule>(sensorRuleId);
            return new SensorRuleMapping().Map(rule);
        }

        List<SensorRule> ISensorRuleService.GetAll()
        {
            var rules = this.repository.Query<Entities.SensorRule>();
            return new SensorRuleMapping().Map(rules).ToList();
        }

        async Task<OperationStatus> ISensorRuleService.Update(SensorRule sensorRule)
        {
            var rule = this.repository.Read<Entities.SensorRule>(sensorRule.Id);

            if (rule == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor rule does not exist for is - {0}.", sensorRule.Id));
            }

            var group = rule.SensorGroup;

            if (group.RuleCreationInProgress)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor rule creation is in progress for group - {0}, can not update rule. Please try after some time.", group.Name));
            }

            if (rule.CapabilityFilterId != sensorRule.CapabilityFilterId)
            {
                var groupCapabilityFilterIds = group.Sensors.SelectMany(s => s.SensorType.Capabilities).SelectMany(c => c.SensorCapabilityFilters).Select(f => f.Id).Distinct();

                var filter = this.repository.Read<Entities.SensorCapabilityFilter>(sensorRule.CapabilityFilterId);

                if (filter == null)
                {
                    return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Capability filter does not exist for id - {0}.", sensorRule.CapabilityFilterId));
                }

                if (!groupCapabilityFilterIds.Any(id => id == sensorRule.CapabilityFilterId))
                {
                    return new OperationStatus(Contracts.Enums.StatusCode.Error, string.Format("Sensor group does not support Capability filter id - {0}.", sensorRule.CapabilityFilterId));
                }

                rule.CapabilityFilterId = sensorRule.CapabilityFilterId;
            }

            rule.MinThreshold = sensorRule.MinThreshold;
            rule.MaxThreshold = sensorRule.MaxThreshold;

            group.RuleCreationInProgress = true;
            this.repository.Persist(group);

            this.repository.Persist(rule);

            await this.groupAlertService.ApplyGroupFilter(rule.SensorGroupId.Value);

            return new OperationStatus();
        }

        async Task<OperationStatus> ISensorRuleService.ResetRules(int groupId)
        {
            var group = this.repository.Read<Entities.SensorGroup>(groupId);

            if (group == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor group does not exist for id - {0}.", groupId));
            }

            if (group.RuleCreationInProgress)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Sensor rule creation is in progress for group - {0}, can not reset rule. Please try after some time.", group.Name));
            }

            var groupCapabilities = group.Sensors.SelectMany(s => s.SensorType.Capabilities).SelectMany(c => c.SensorCapabilityFilters.Select(f => f.Id));

            var rules = group.SensorRules.Where(r => !groupCapabilities.Any(id => id == r.CapabilityFilterId));

            if (rules.Count() > 0)
            {
                await this.alertService.DeleteAlerts(groupId, rules.Select(r => r.Id).ToList());

                foreach (var rule in rules.ToList())
                {
                    this.repository.Delete(rule);
                }

                group.RuleCreationInProgress = true;
                this.repository.Persist(group);

                await this.groupAlertService.ApplyGroupFilter(groupId);
            }

            return new OperationStatus();
        }
    }
}
