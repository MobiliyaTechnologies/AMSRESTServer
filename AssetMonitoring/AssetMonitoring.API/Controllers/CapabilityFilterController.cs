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
    /// Provides capability filter APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class CapabilityFilterController : ApiController
    {
        private readonly ISensorCapabilityFilterService sensorCapabilityFilterService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CapabilityFilterController"/> class.
        /// </summary>
        /// <param name="sensorCapabilityFilterService">The sensor capability filter service.</param>
        public CapabilityFilterController(ISensorCapabilityFilterService sensorCapabilityFilterService)
        {
            this.sensorCapabilityFilterService = sensorCapabilityFilterService;
        }

        /// <summary>
        /// Gets all capability filters.
        /// </summary>
        /// <returns>The capability filter details.</returns>
        public List<CapabilityFilter> GetAll()
        {
            var filters = this.sensorCapabilityFilterService.GetAll();
            return filters;
        }

        /// <summary>
        /// Gets all filters by capability.
        /// </summary>
        /// <param name="capabilityId">The capability identifier.</param>
        /// <returns>
        /// The capability filters.
        /// </returns>
        [Route("GetAllFilterByCapability/{capabilityId}")]
        public List<CapabilityFilter> GetAllFilterByCapability(int capabilityId)
        {
            var capabilityFilters = this.sensorCapabilityFilterService.GetAllFilterByCapability(capabilityId);
            return capabilityFilters;
        }

        /// <summary>
        /// Gets the specified capability filter.
        /// </summary>
        /// <param name="id">The capability filter identifier.</param>
        /// <returns>The capability filter detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(CapabilityFilter))]
        public IHttpActionResult Get(int id)
        {
            var filter = this.sensorCapabilityFilterService.Get(id);

            if (filter == null)
            {
                return this.NotFound();
            }

            return this.Ok(filter);
        }

        /// <summary>
        /// Creates the specified capability filter.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="capabilityFilter">The capability filter.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Post(CapabilityFilter capabilityFilter)
        {
            if (capabilityFilter == null && capabilityFilter.CapabilityId < 1)
            {
                return this.BadRequest("Invalid capability filter model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.sensorCapabilityFilterService.Create(capabilityFilter);

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
        /// Updates the specified capability filter.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="capabilityFilter">The capability filter.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Put(CapabilityFilter capabilityFilter)
        {
            if (capabilityFilter == null || capabilityFilter.Id < 1 || capabilityFilter.CapabilityId < 1)
            {
                return this.BadRequest("Invalid capability filter model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.sensorCapabilityFilterService.Update(capabilityFilter);

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
        /// Deletes the specified capability filter.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="id">The capability filter identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Capability filter id must be grater than 0.");
            }

            var operationStatus = this.sensorCapabilityFilterService.Delete(id);

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
