namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;

    /// <summary>
    /// Provides sensor type related operations.
    /// </summary>
    public interface ISensorTypeService
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The sensor types.</returns>
        List<SensorType> GetAll();

        /// <summary>
        /// Gets the specified sensor type identifier.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>The sensor type.</returns>
        SensorType Get(int sensorTypeId);

        /// <summary>
        /// Creates the specified sensor type.
        /// </summary>
        /// <param name="sensorType">Type of the sensor.</param>
        /// <returns>The creation status.</returns>
        Task<OperationStatus> Create(SensorType sensorType);

        /// <summary>
        /// Updates the specified sensor type.
        /// </summary>
        /// <param name="sensorType">Type of the sensor.</param>
        /// <returns>The update status.</returns>
        OperationStatus Update(SensorType sensorType);

        /// <summary>
        /// Deletes the specified sensor type identifier.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>The delete status.</returns>
        Task<OperationStatus> Delete(int sensorTypeId);

        /// <summary>
        /// Adds the type of the capabilities to sensor.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <param name="capabilityIds">The capability ids.</param>
        /// <returns>
        /// The sensor type and capabilities association status.
        /// </returns>
        Task<OperationStatus> SetCapabilitiesToSensorType(int sensorTypeId, List<int> capabilityIds);
    }
}
