namespace AssetMonitoring.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides group alert related operations.
    /// </summary>
    public interface IGroupAlertService
    {
        /// <summary>
        /// Applies the group filter.
        /// Creates stream analytics job for given group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        Task ApplyGroupFilter(int groupId);

        /// <summary>
        /// Deletes the group filter.
        /// Delete stream analytics jobs for given group identifier.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        Task DeleteGroupFilter(int groupId);
    }
}
