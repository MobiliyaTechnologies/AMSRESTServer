namespace AssetMonitoring.Services
{
    using System.Collections.Generic;
    using AssetMonitoring.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides sensor ruler related operations.
    /// </summary>
    public interface ISensorRuleService
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The sensor rules.</returns>
        List<SensorRule> GetAll();

        /// <summary>
        /// Gets the specified sensor rule identifier.
        /// </summary>
        /// <param name="sensorRuleId">The sensor rule identifier.</param>
        /// <returns>The sensor.</returns>
        SensorRule Get(int sensorRuleId);

        /// <summary>
        /// Creates the sensor rules for given sensor group.
        /// </summary>
        /// <param name="sensorGroupId">The sensor group identifier.</param>
        /// <param name="sensorRules">The sensor rules.</param>
        /// <returns>
        /// The create status.
        /// </returns>
        Task<OperationStatus> Create(int sensorGroupId, List<SensorRule> sensorRules);

        /// <summary>
        /// Updates the specified sensor rule.
        /// </summary>
        /// <param name="sensorRule">The sensor rule.</param>
        /// <returns>The update status.</returns>
        Task<OperationStatus> Update(SensorRule sensorRule);

        /// <summary>
        /// Deletes the specified sensor rule identifier.
        /// </summary>
        /// <param name="sensorRuleId">The sensor rule identifier.</param>
        /// <returns>The delete status.</returns>
        Task<OperationStatus> Delete(int sensorRuleId);

        /// <summary>
        /// Resets the group rules.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The reset status.</returns>
        Task<OperationStatus> ResetRules(int groupId);
    }
}
