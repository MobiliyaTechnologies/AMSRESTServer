using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetMonitoring.Contracts;

namespace AssetMonitoring.Services
{
    /// <summary>
    /// Provides operations to perform CRUD on application configuration.
    /// </summary>
    public interface IApplicationConfigurationService
    {
        /// <summary>
        /// Gets all application configuration.
        /// </summary>
        /// <returns>The application configuration list.</returns>
        List<Configuration> GetAllApplicationConfiguration();

        /// <summary>
        /// Gets the application configuration for given configuration type..
        /// </summary>
        /// <param name="applicationConfigurationType">Type of the application configuration.</param>
        /// <returns>The application configuration.</returns>
        Configuration GetApplicationConfiguration(string applicationConfigurationType);

        /// <summary>
        /// Adds the application configuration.
        /// </summary>
        /// <param name="applicationConfiguration">The application configuration model.</param>
        /// <returns>Application configuration added confirmation.</returns>
        OperationStatus AddApplicationConfiguration(Configuration applicationConfiguration);

        /// <summary>
        /// Deletes the application configuration.
        /// </summary>
        /// <param name="applicationConfigurationType">Type of the application configuration.</param>
        /// <returns>Application configuration deleted confirmation.</returns>
        OperationStatus DeleteApplicationConfiguration(string applicationConfigurationType);

        /// <summary>
        /// Updates the application configuration entry.
        /// </summary>
        /// <param name="applicationConfigurationEntry">The application configuration entry model.</param>
        /// <returns>Application configuration entry update confirmation.</returns>
        OperationStatus UpdateApplicationConfigurationEntry(ApplicationConfigurationEntry applicationConfigurationEntry);

        /// <summary>
        /// Deletes the application configuration entry.
        /// </summary>
        /// <param name="applicationConfigurationEntryId">The application configuration entry identifier.</param>
        /// <returns>Application configuration entry deleted confirmation.</returns>
        OperationStatus DeleteApplicationConfigurationEntry(int applicationConfigurationEntryId);
    }
}
