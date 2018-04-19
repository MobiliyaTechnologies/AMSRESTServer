namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using System.Web.Routing;
    using AssetMonitoring.API.HttpFilters;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services;

    /// <summary>
    /// Provides gateway APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class GatewayController : ApiController
    {
        private readonly IGatewayService gatewayService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayController"/> class.
        /// </summary>
        /// <param name="gatewayService">The gateway service.</param>
        public GatewayController(IGatewayService gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        /// <summary>
        /// Gets all gateways.
        /// </summary>
        /// <returns>The gateway details</returns>
        public List<Gateway> GetAll()
        {
            var gatways = this.gatewayService.GetAll();
            return gatways;
        }

        /// <summary>
        /// Gets all online gateways.
        /// </summary>
        /// <returns>The gateway details</returns>
        [Route("OnlineGateway")]
        public async Task<List<Gateway>> GetAllOnlineGateway()
        {
            var gatways = await this.gatewayService.GetAllOnlineGateway();
            return gatways;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The gateway detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(Gateway))]
        public IHttpActionResult Get(int id)
        {
            var gateway = this.gatewayService.Get(id);

            if (gateway == null)
            {
                return this.NotFound();
            }

            return this.Ok(gateway);
        }

        /// <summary>
        /// Gets the gateway iot hub connection details.
        /// </summary>
        /// <param name="gatewayKey">The gateway key.</param>
        /// <returns>
        /// The gateway detail if found else NotFound(404) status code.
        /// </returns>
        [ResponseType(typeof(IotHubGateway))]
        [Route("IotHubGateway")]
        [HttpPost]
        public async Task<IHttpActionResult> GetIotHubGateway([FromBody]string gatewayKey)
        {
            if (string.IsNullOrWhiteSpace(gatewayKey))
            {
                return this.BadRequest("Invalid gateway key.");
            }

            var gateway = await this.gatewayService.GetIotHubGateway(gatewayKey);

            if (gateway == null)
            {
                return this.NotFound();
            }

            return this.Ok(gateway);
        }

        /// <summary>
        /// Creates the specified gateway.
        /// GatewayId must be unique and continuous string, white space not allowed in between words.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <returns>
        /// The created(201) on successfully creation else BadRequest(400) status code.
        /// </returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Post(Gateway gateway)
        {
            if (gateway == null)
            {
                return this.BadRequest("Invalid gateway model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.gatewayService.Create(gateway);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Ok)
            {
                return this.StatusCode(HttpStatusCode.Created);
            }
            else
            {
                return this.BadRequest(operationStatus.Message);
            }
        }

        /// <summary>
        /// Updates the specified gateway.
        /// Only name and description allowed to modify.
        /// To modify GatewayId delete existing and create new.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <returns>
        /// The updated(204) on successfully updation else BadRequest(400) status code.
        /// </returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Put(Gateway gateway)
        {
            if (gateway == null || gateway.Id < 1)
            {
                return this.BadRequest("Invalid gateway model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.gatewayService.Update(gateway);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Ok)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return this.BadRequest(operationStatus.Message);
            }
        }

        /// <summary>
        /// Deletes the specified gateway.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="id">The gateway identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Sensor capability id must be grater than 0.");
            }

            var operationStatus = await this.gatewayService.Delete(id);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Ok)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return this.BadRequest(operationStatus.Message);
            }
        }
    }
}
