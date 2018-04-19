namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;

    /// <summary>
    /// Provides sensor related operations.
    /// </summary>
    public interface ISensorService
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The sensors.</returns>
        List<Sensor> GetAll();

        /// <summary>
        /// Gets all sensors by sensor type.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>The sensors</returns>
        List<Sensor> GetAllSensorBySensorType(int sensorTypeId);

        /// <summary>
        /// Gets all unmapped sensors.
        /// </summary>
        /// <returns>The sensors not associated with any group.</returns>
        List<Sensor> GetAllUnmappedSensors();

        /// <summary>
        /// Gets the specified sensor identifier.
        /// </summary>
        /// <param name="sensorId">The sensor identifier.</param>
        /// <returns>The sensor.</returns>
        Sensor Get(int sensorId);

        /// <summary>
        /// Creates the specified sensor.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        /// <returns>The create status.</returns>
        OperationStatus Create(Sensor sensor);

        /// <summary>
        /// Updates the specified sensor.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        /// <returns>The update status.</returns>
        OperationStatus Update(Sensor sensor);

        /// <summary>
        /// Deletes the specified sensor identifier.
        /// </summary>
        /// <param name="sensorId">The sensor identifier.</param>
        /// <returns>The delete status.</returns>
        Task<OperationStatus> Delete(int sensorId);

        /// <summary>
        /// Gets the sensor type for given sensor key.
        /// </summary>
        /// <param name="sensorKey">The sensor key.</param>
        /// <returns>The sensor type.</returns>
        string GetSensorType(string sensorKey);

        OperationStatus AddSensorToSensorType(int sensorTypeId, List<Sensor> sensors);
    }
}
