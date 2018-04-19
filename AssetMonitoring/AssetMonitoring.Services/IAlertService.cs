namespace AssetMonitoring.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using Contracts;

    public interface IAlertService
    {
        /// <summary>
        /// Gets the alert by group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The alert details</returns>
       List<AlertDocument> GetAlertByGroup(int groupId);

        /// <summary>
        /// Gets all alert details.
        /// </summary>
        /// <param name="alertPaginationFilter">The alert pagination filter.</param>
        /// <returns>
        /// The alert details.
        /// </returns>
        Task<AlertPaginationResult> GetPaginateAlert(AlertPaginationFilter alertPaginationFilter);

        /// <summary>
        /// Gets the distinct damage asset barcodes.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>
        /// The damage asset barcodes.
        /// </returns>
        List<string> GetDamageAssetBarcodes(int? groupId = null);

        /// <summary>
        /// Gets the alerts for given group id and asset barcode..
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <returns>The alert details.</returns>
        List<AlertDocument> GetAlerts(int groupId, string assetBarcode);

        /// <summary>
        /// Deletes the alerts.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="sensorRuleIds">The sensor rule ids.</param>
        Task DeleteAlerts(int groupId, List<int> sensorRuleIds = null);

        /// <summary>
        /// Gets the group alert filter.
        /// </summary>
        /// <returns>The group alert filter.</returns>
        List<GroupAlertPaginateFilter> GetGroupAlertFilter();
    }
}
