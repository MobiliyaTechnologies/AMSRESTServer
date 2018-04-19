namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;

    public interface IAssetService
    {
        /// <summary>
        /// Gets all assets.
        /// </summary>
        /// <returns>The group asset details.</returns>
        List<GroupAsset> GetAll();

        /// <summary>
        /// Gets the asset for given asset barcode.
        /// </summary>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <returns>
        /// The group asset detail.
        /// </returns>
        GroupAsset Get(string assetBarcode);

        /// <summary>
        /// Creates the specified asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>The creation status.</returns>
        Task<OperationStatus> Create(Asset asset);

        /// <summary>
        /// Detaches the sensor from asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>
        /// The detach status.
        /// </returns>
        Task<OperationStatus> DetachSensor(Asset asset);

        /// <summary>
        /// Deletes the specified asset identifier.
        /// </summary>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <returns>
        /// The deletion status.
        /// </returns>
        Task<OperationStatus> Delete(string assetBarcode);

        /// <summary>
        /// Gets the asset status.
        /// </summary>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <param name="operationStatus">The operation status.</param>
        /// <returns>
        /// The asset status.
        /// </returns>
        List<AssetStatus> GetAssetStatus(string assetBarcode, OperationStatus operationStatus);

        /// <summary>
        /// Gets all damage asset.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The group asset details.</returns>
        List<GroupAsset> GetAllDamagAsset(int? groupId = null);
    }
}
