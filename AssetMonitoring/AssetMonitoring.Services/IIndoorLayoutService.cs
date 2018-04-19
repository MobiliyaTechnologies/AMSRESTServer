namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using AssetMonitoring.Contracts;

    /// <summary>
    /// Provides indoor layout related operations.
    /// </summary>
    public interface IIndoorLayoutService
    {
        /// <summary>
        /// Gets all indoor layouts.
        /// </summary>
        /// <returns>The indoor layouts.</returns>
        List<IndoorLayout> GetAll();

        /// <summary>
        /// Gets the specified indoor layout identifier.
        /// </summary>
        /// <param name="indoorLayoutId">The indoor layout identifier.</param>
        /// <returns>The indoor layout.</returns>
        IndoorLayout Get(int indoorLayoutId);

        /// <summary>
        /// Adds the specified indoor layout.
        /// </summary>
        /// <param name="indoorLayout">The indoor layout.</param>
        void Add(IndoorLayout indoorLayout);

        /// <summary>
        /// Deletes the specified indoor layout identifier.
        /// </summary>
        /// <param name="indoorLayoutId">The indoor layout identifier.</param>
        /// <returns>The operation status.</returns>
        OperationStatus Delete(int indoorLayoutId);

        /// <summary>
        /// Maps the gateway to given indoor layout.
        /// </summary>
        /// <param name="indoorLayout">The indoor layout.</param>
        /// <returns>The operation status.</returns>
        OperationStatus MapGateway(IndoorLayout indoorLayout);

        /// <summary>
        /// Detaches the gateway from given indoor layout.
        /// </summary>
        /// <param name="indoorLayout">The indoor layout.</param>
        /// <returns>The operation status.</returns>
        OperationStatus DetachGateway(IndoorLayout indoorLayout);
    }
}
