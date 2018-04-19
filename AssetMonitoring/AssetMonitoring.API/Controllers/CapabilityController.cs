namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.API.HttpFilters;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services;

    /// <summary>
    /// Provides capability APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class CapabilityController : ApiController
    {
        private readonly ICapabilityService sensorCapabilityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CapabilityController"/> class.
        /// </summary>
        /// <param name="sensorCapabilityService">The sensor capability service.</param>
        public CapabilityController(ICapabilityService sensorCapabilityService)
        {
            this.sensorCapabilityService = sensorCapabilityService;
        }

        /// <summary>
        /// Gets all capabilities.
        /// </summary>
        /// <returns>The capability details.</returns>
        public List<Capability> GetAll()
        {
            var capabilities = this.sensorCapabilityService.GetAll();
            return capabilities;
        }

        /// <summary>
        /// Gets all sensor type capabilities.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>
        /// The sensor type capabilities.
        /// </returns>
        [Route("SensorTypeCapabilities/{sensorTypeId}")]
        public List<Capability> GetAllSensorTypeCapabilities(int sensorTypeId)
        {
            var sensorTypeCapabilities = this.sensorCapabilityService.GetAllSensorTypeCapabilities(sensorTypeId);
            return sensorTypeCapabilities;
        }

        /// <summary>
        /// Gets the specified capability.
        /// </summary>
        /// <param name="id">The capability identifier.</param>
        /// <returns>The capability detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(Capability))]
        public IHttpActionResult Get(int id)
        {
            var capability = this.sensorCapabilityService.Get(id);

            if (capability == null)
            {
                return this.NotFound();
            }

            return this.Ok(capability);
        }

        /// <summary>
        /// Creates the specified capability.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Post(Capability capability)
        {
            if (capability == null)
            {
                return this.BadRequest("Invalid capability model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.sensorCapabilityService.Create(capability);

            return this.StatusCode(HttpStatusCode.Created);
        }

        /// <summary>
        /// Updates the specified capabilities.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="capabilities">The capabilities collection to be update.</param>
        /// <returns>
        /// The updated(204) on successfully updation else BadRequest(400) status code.
        /// </returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Put(List<Capability> capabilities)
        {
            if (capabilities == null || capabilities.Count < 1)
            {
                return this.BadRequest("Invalid capability model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            foreach (var capability in capabilities)
            {
                var operationStatus = this.sensorCapabilityService.Update(capability);

                if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Error)
                {
                    return this.BadRequest(operationStatus.Message);
                }
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes the specified capability.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="id">The capability identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Sensor capability id must be grater than 0.");
            }

            var operationStatus = this.sensorCapabilityService.Delete(id);

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
