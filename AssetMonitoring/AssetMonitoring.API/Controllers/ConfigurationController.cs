namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AssetMonitoring.Components.Repository;

    /// <summary>
    /// Provides configuration APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class ConfigurationController : ApiController
    {
        /// <summary>
        /// Gets the B2C configuration.
        /// </summary>
        /// <returns>The b2c configuration dictionary.</returns>
        [Route("GetMobileConfiguration")]
        [AllowAnonymous]
        public IDictionary<string, string> GetMobileConfiguration()
        {
            var b2cConfiguration = new Dictionary<string, string>();

            b2cConfiguration.Add("B2cMobileRedirectUrl", Utilities.ApplicationConfiguration.B2cMobileRedirectUrl);
            b2cConfiguration.Add("B2cTenant", Utilities.ApplicationConfiguration.B2cTenant);
            b2cConfiguration.Add("B2cClientId", Utilities.ApplicationConfiguration.B2cClientId);
            b2cConfiguration.Add("B2cSignUpInPolicyId", Utilities.ApplicationConfiguration.B2cSignUpInPolicyId);
            b2cConfiguration.Add("B2cMobileInstanceUrl", Utilities.ApplicationConfiguration.B2cMobileInstanceUrl);
            b2cConfiguration.Add("B2cMobileTokenUrl", Utilities.ApplicationConfiguration.B2cMobileTokenUrl);

            return b2cConfiguration;
        }

        /// <summary>
        /// Gets the simulator B2C configuration.
        /// </summary>
        /// <returns>The simulator configuration dictionary.</returns>
        [Route("GetB2cConfiguration")]
        [AllowAnonymous]
        public IDictionary<string, string> GetSimulatorB2cConfiguration()
        {
            var b2cConfiguration = new Dictionary<string, string>();

            b2cConfiguration.Add("B2cTenant", Utilities.ApplicationConfiguration.B2cTenant);
            b2cConfiguration.Add("B2cClientId", Utilities.ApplicationConfiguration.B2cClientId);
            b2cConfiguration.Add("B2cSignUpInPolicyId", Utilities.ApplicationConfiguration.B2cSignUpInPolicyId);

            return b2cConfiguration;
        }

        /// <summary>
        /// Gets the simulator app configuration.
        /// </summary>
        /// <returns>The simulator configuration dictionary.</returns>
        [Route("GetSimulatorAppConfiguration")]
        public async Task<IDictionary<string, string>> GetSimulatorAppConfiguration()
        {
            var b2cConfiguration = new Dictionary<string, string>();

            b2cConfiguration.Add("SimulatorDbServer", Utilities.ApplicationConfiguration.SimulatorDbServer);
            b2cConfiguration.Add("SimulatorDbName", Utilities.ApplicationConfiguration.SimulatorDbName);
            b2cConfiguration.Add("SimulatorDbUserId", Utilities.ApplicationConfiguration.SimulatorDbUserId);
            b2cConfiguration.Add("SimulatorDbPassword", Utilities.ApplicationConfiguration.SimulatorDbPassword);

            b2cConfiguration.Add("IotHubConnectionString", Utilities.ApplicationConfiguration.IotHubConnectionString);
            b2cConfiguration.Add("IotHubHostName", Utilities.ApplicationConfiguration.IotHubHostName);

            return b2cConfiguration;
        }
    }
}
