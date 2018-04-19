namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.API.HttpFilters;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services;

    /// <summary>
    /// Provides sensor type APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class SensorTypeController : ApiController
    {
        private readonly ISensorTypeService sensorTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorTypeController"/> class.
        /// </summary>
        /// <param name="sensorTypeService">The sensor type service.</param>
        public SensorTypeController(ISensorTypeService sensorTypeService)
        {
            this.sensorTypeService = sensorTypeService;
        }

        /// <summary>
        /// Gets all sensor types.
        /// </summary>
        /// <returns>The sensor type details.</returns>
        public List<SensorType> GetAll()
        {
            var sensorTypes = this.sensorTypeService.GetAll();
            return sensorTypes;
        }

        /// <summary>
        /// Gets the specified sensor type.
        /// </summary>
        /// <param name="id">The sensor type identifier.</param>
        /// <returns>The sensor type detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(SensorType))]
        public IHttpActionResult Get(int id)
        {
            var sensorType = this.sensorTypeService.Get(id);

            if (sensorType == null)
            {
                return this.NotFound();
            }

            return this.Ok(sensorType);
        }

        /// <summary>
        /// Creates the specified sensor type.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="sensorType">Type of the sensor.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Post(SensorType sensorType)
        {
            if (sensorType == null)
            {
                return this.BadRequest("Invalid sensor type model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.sensorTypeService.Create(sensorType);

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
        /// Updates the specified sensor type.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="sensorType">Type of the sensor.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public IHttpActionResult Put(SensorType sensorType)
        {
            if (sensorType == null || sensorType.Id < 1)
            {
                return this.BadRequest("Invalid sensor type model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.sensorTypeService.Update(sensorType);

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
        /// Deletes the specified sensor type.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="id">The sensor type identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Sensor type id must be grater than 0.");
            }

            var operationStatus = await this.sensorTypeService.Delete(id);

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
        /// Sets the capabilities to sensor type.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <param name="capabilityIds">The capability ids.</param>
        /// <returns>The success(204) on successfully association else BadRequest(400) status code.</returns>
        [HttpPut]
        [ResponseType(typeof(StatusCodeResult))]
        [Route("SetCapabilitiesToSensorType/{sensorTypeId}")]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> SetCapabilitiesToSensorType(int sensorTypeId, [FromBody] List<int> capabilityIds)
        {
            if (sensorTypeId < 1 || capabilityIds.Count == 0 || capabilityIds.Any(i => i < 1))
            {
                return this.BadRequest("Invalid Inputs");
            }

            var operationStatus = await this.sensorTypeService.SetCapabilitiesToSensorType(sensorTypeId, capabilityIds);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Error)
            {
                return this.BadRequest(operationStatus.Message);
            }
            else
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }
        }
    }
}