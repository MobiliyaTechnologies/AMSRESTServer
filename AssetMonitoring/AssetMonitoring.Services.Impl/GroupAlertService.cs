namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Context;
    using AssetMonitoring.Contracts.AnalyticsContract;
    using Contracts;
    using Utilities;

    public class GroupAlertService : IGroupAlertService
    {
        private readonly IQueueStorageService queueStorageService;

        public GroupAlertService(IQueueStorageService queueStorageService)
        {
            this.queueStorageService = queueStorageService;
        }

        async Task IGroupAlertService.ApplyGroupFilter(int groupId)
        {
            await this.queueStorageService.SendMessage<int>(ApplicationConstant.CreateRuleJobQueueName, groupId);
        }

        async Task IGroupAlertService.DeleteGroupFilter(int groupId)
        {
            await this.queueStorageService.SendMessage<int>(ApplicationConstant.DeleteRuleJobQueueName, groupId);
        }
    }
}
