namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;

    /// <summary>
    /// Provides gateway related operation.
    /// </summary>
    public interface IGatewayService
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The gateways.</returns>
        List<Gateway> GetAll();

        /// <summary>
        /// Gets all gateway connected to IoT hub.
        /// </summary>
        /// <returns>The gateways.</returns>
        Task<List<Gateway>> GetAllOnlineGateway();

        /// <summary>
        /// Gets the specified gateway for given identifier.
        /// </summary>
        /// <param name="gatewayId">The gateway identifier.</param>
        /// <returns>The gateway.</returns>
        Gateway Get(int gatewayId);

        /// <summary>
        /// Gets the gateway with iot hub connection details for given identifier.
        /// </summary>
        /// <param name="gatewayKey">The gateway key.</param>
        /// <returns>
        /// The gateway.
        /// </returns>
        Task<IotHubGateway> GetIotHubGateway(string gatewayKey);

        /// <summary>
        /// Creates the specified gateway.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        Task<OperationStatus> Create(Gateway gateway);

        /// <summary>
        /// Updates the specified gateway.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <returns>The update status.</returns>
        Task<OperationStatus> Update(Gateway gateway);

        /// <summary>
        /// Deletes the specified gateway identifier.
        /// </summary>
        /// <param name="gatewayId">The gateway identifier.</param>
        /// <returns>The delete status.</returns>
        Task<OperationStatus> Delete(int gatewayId);

        Task GatewayMessage<T>(DeviceMessageStatus status, List<T> messageData, List<string> gatewayKeys = null, bool allowDisconnectedDevice = false);
    }
}
