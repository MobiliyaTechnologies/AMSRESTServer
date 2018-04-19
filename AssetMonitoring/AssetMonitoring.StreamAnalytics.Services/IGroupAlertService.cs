namespace AssetMonitoring.StreamAnalytics.Services
{
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.AnalyticsContract;

    /// <summary>
    /// Provides group related operation.
    /// </summary>
    public interface IGroupAlertService
    {
        /// <summary>
        /// Applies the group rule.
        /// Creates stream analytics job for group rules.
        /// </summary>
        /// <param name="groupAlertFilter">The group alert filter.</param>
        Task ApplyGroupRule(GroupAlertFilter groupAlertFilter);

        /// <summary>
        /// Deletes the stream analytics job for given group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        Task DeleteGroup(int groupId);
    }
}
