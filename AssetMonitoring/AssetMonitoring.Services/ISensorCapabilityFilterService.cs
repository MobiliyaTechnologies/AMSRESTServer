namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using AssetMonitoring.Contracts;

    /// <summary>
    /// Provides sensor sensorCapabilityFilter filter related operations.
    /// </summary>
    public interface ISensorCapabilityFilterService
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>
        /// The sensor capability filters.
        /// </returns>
        List<CapabilityFilter> GetAll();

        /// <summary>
        /// Gets all filter by capability.
        /// </summary>
        /// <param name="capabilityId">The capability identifier.</param>
        /// <returns> The sensor capability filters.</returns>
        List<CapabilityFilter> GetAllFilterByCapability(int capabilityId);

        /// <summary>
        /// Gets the specified sensor capability filter identifier.
        /// </summary>
        /// <param name="sensorCapabilityFilterId">The sensor capability identifier.</param>
        /// <returns>
        /// The sensor capability filter.
        /// </returns>
        CapabilityFilter Get(int sensorCapabilityFilterId);

        /// <summary>
        /// Creates the specified sensor capability filter.
        /// </summary>
        /// <param name="sensorCapabilityFilter">The sensor capability filter.</param>
        /// <returns>
        /// The create status.
        /// </returns>
        OperationStatus Create(CapabilityFilter sensorCapabilityFilter);

        /// <summary>
        /// Updates the specified sensor capability filter.
        /// </summary>
        /// <param name="sensorCapabilityFilter">The sensor capability filter.</param>
        /// <returns>
        /// The update status.
        /// </returns>
        OperationStatus Update(CapabilityFilter sensorCapabilityFilter);

        /// <summary>
        /// Deletes the specified sensor capability filter identifier.
        /// </summary>
        /// <param name="sensorCapabilityFilterId">The sensor capability filter identifier.</param>
        /// <returns>
        /// The delete status.
        /// </returns>
        OperationStatus Delete(int sensorCapabilityFilterId);
    }
}
