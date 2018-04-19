namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.DocumentDbContract;

    public interface ISensorGroupService
    {
        /// <summary>
        /// Gets all sensor groups.
        /// </summary>
        /// <returns>The sensor group details.</returns>
        List<SensorGroup> GetAll();

        /// <summary>
        /// Gets the specified sensor group identifier.
        /// </summary>
        /// <param name="sensorGroupId">The sensor group identifier.</param>
        /// <returns>The sensor group detail.</returns>
        SensorGroup Get(int sensorGroupId);

        /// <summary>
        /// Creates the specified sensor group.
        /// </summary>
        /// <param name="sensorGroup">The sensor group.</param>
        /// <returns>The create status.</returns>
        Task<OperationStatus> Create(SensorGroup sensorGroup);

        /// <summary>
        /// Updates the specified sensor group.
        /// </summary>
        /// <param name="sensorGroup">The sensor group.</param>
        /// <returns>The update status.</returns>
        OperationStatus Update(SensorGroup sensorGroup);

        /// <summary>
        /// Deletes the specified sensor group identifier.
        /// </summary>
        /// <param name="sensorGroupId">The sensor group identifier.</param>
        /// <returns>The delete status.</returns>
        Task<OperationStatus> Delete(int sensorGroupId);

        /// <summary>
        /// Adds the asset to group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="assetIds">The asset ids.</param>
        /// <returns>The addition status.</returns>
        Task<OperationStatus> AddAsset(int groupId, List<int> assetIds);

        /// <summary>
        /// Removes the asset.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="assetIds">The asset ids.</param>
        /// <returns>The removal status.</returns>
        Task<OperationStatus> RemoveAsset(int groupId, List<int> assetIds);

        /// <summary>
        /// Detaches the sensors.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The detach status.</returns>
        Task<OperationStatus> DetachSensors(int groupId);

        /// <summary>
        /// Gets the GPS location of all group.
        /// </summary>
        /// <returns>The group location.</returns>
        Task<List<GroupGpsDetail>> GetAllGroupStartEndLocation();
    }
}
