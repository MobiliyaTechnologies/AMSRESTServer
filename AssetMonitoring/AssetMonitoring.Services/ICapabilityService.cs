namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using AssetMonitoring.Contracts;

    /// <summary>
    /// Provides capability related operations.
    /// </summary>
    public interface ICapabilityService
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>
        /// The capabilities.
        /// </returns>
        List<Capability> GetAll();

        /// <summary>
        /// Gets all capabilities for given sensor type identifier.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>The sensor type capabilities.</returns>
        List<Capability> GetAllSensorTypeCapabilities(int sensorTypeId);

        /// <summary>
        /// Gets the specified capability identifier.
        /// </summary>
        /// <param name="capabilityId">The capability identifier.</param>
        /// <returns>
        /// The capability.
        /// </returns>
        Capability Get(int capabilityId);

        /// <summary>
        /// Creates the specified capability.
        /// </summary>
        /// <param name="capability">The capability.</param>
        void Create(Capability capability);

        /// <summary>
        /// Updates the specified capability.
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns>
        /// The update status.
        /// </returns>
        OperationStatus Update(Capability capability);

        /// <summary>
        /// Deletes the specified capability identifier.
        /// </summary>
        /// <param name="capabilityId">The capability identifier.</param>
        /// <returns>
        /// The delete status.
        /// </returns>
        OperationStatus Delete(int capabilityId);
    }
}
