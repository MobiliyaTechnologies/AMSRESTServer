namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Services;
    using System.Linq;
    using AssetMonitoring.Contracts.DocumentDbContract;

    /// <summary>
    /// Provides sensor group APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class SensorGroupController : ApiController
    {
        private readonly ISensorGroupService sensorGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorGroupController"/> class.
        /// </summary>
        /// <param name="sensorGroupService">The sensor group service.</param>
        public SensorGroupController(ISensorGroupService sensorGroupService)
        {
            this.sensorGroupService = sensorGroupService;
        }

        /// <summary>
        /// Gets all sensor groups.
        /// Returns only group details excludes sensors, rules and capability filters.
        /// </summary>
        /// <returns>The sensor group details.</returns>
        public List<SensorGroup> GetAll()
        {
            var sensorGroups = this.sensorGroupService.GetAll();
            return sensorGroups;
        }

        /// <summary>
        /// Gets the specified sensor group.
        /// </summary>
        /// <param name="id">The sensor group identifier.</param>
        /// <returns>The sensor group detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(SensorGroup))]
        public IHttpActionResult Get(int id)
        {
            var sensor = this.sensorGroupService.Get(id);

            if (sensor == null)
            {
                return this.NotFound();
            }

            return this.Ok(sensor);
        }

        /// <summary>
        /// Creates the specified sensor group.
        /// </summary>
        /// <param name="sensorGroup">The sensor group.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Post(SensorGroup sensorGroup)
        {
            if (sensorGroup == null)
            {
                return this.BadRequest("Invalid sensor group model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.sensorGroupService.Create(sensorGroup);

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
        /// Updates the specified sensor group.
        /// </summary>
        /// <param name="sensorGroup">The sensor group.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Put(SensorGroup sensorGroup)
        {
            if (sensorGroup == null || sensorGroup.Id < 1)
            {
                return this.BadRequest("Invalid sensor group model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.sensorGroupService.Update(sensorGroup);

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
        /// Deletes the specified sensor group.
        /// </summary>
        /// <param name="id">The sensor group identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Group id must be grater than 0.");
            }

            var operationStatus = await this.sensorGroupService.Delete(id);

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
        /// Adds the assets to group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="assetIds">The asset ids.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [HttpPut]
        [Route("AddAsset/{groupId}")]
        public async Task<IHttpActionResult> AddAsset(int groupId, List<int> assetIds)
        {
            if (groupId < 1)
            {
                return this.BadRequest("Group id must be grater than 0.");
            }

            if (assetIds == null || assetIds.Count < 1 || assetIds.Any(id => id < 1))
            {
                return this.BadRequest("Invalid asset ids.");
            }

            var operationStatus = await this.sensorGroupService.AddAsset(groupId, assetIds);

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
        /// Removes the assets to group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="assetIds">The asset ids.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [HttpPut]
        [Route("RemoveAsset/{groupId}")]
        public async Task<IHttpActionResult> RemoveAsset(int groupId, List<int> assetIds)
        {
            if (groupId < 1)
            {
                return this.BadRequest("Group id must be grater than 0.");
            }

            if (assetIds == null || assetIds.Count < 1 || assetIds.Any(id => id < 1))
            {
                return this.BadRequest("Invalid asset ids.");
            }

            var operationStatus = await this.sensorGroupService.RemoveAsset(groupId, assetIds);

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
        /// Detaches the sensors.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>
        /// The updated(204) on successfully updation else BadRequest(400) status code.
        /// </returns>
        [ResponseType(typeof(StatusCodeResult))]
        [HttpPut]
        [Route("DetachSensors/{groupId}")]
        public async Task<IHttpActionResult> DetachSensors(int groupId)
        {
            if (groupId < 1)
            {
                return this.BadRequest("Group id must be grater than 0.");
            }

            var operationStatus = await this.sensorGroupService.DetachSensors(groupId);

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
        /// Gets all group start end location.
        /// </summary>
        /// <returns>The group location details.</returns>
        [Route("GetAllGroupStartEndLocation")]
        public async Task<List<GroupGpsDetail>> GetAllGroupStartEndLocation()
        {
            var groupGpsDetails = await this.sensorGroupService.GetAllGroupStartEndLocation();
            return groupGpsDetails;
        }
    }
}
