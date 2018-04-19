namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Services;

    /// <summary>
    /// Provide sensor rule APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class SensorRuleController : ApiController
    {
        private readonly ISensorRuleService sensorRuleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorRuleController"/> class.
        /// </summary>
        /// <param name="sensorRuleService">The sensor rule service.</param>
        public SensorRuleController(ISensorRuleService sensorRuleService)
        {
            this.sensorRuleService = sensorRuleService;
        }

        /// <summary>
        /// Gets the specified sensor rule.
        /// </summary>
        /// <param name="id">The sensor rule identifier.</param>
        /// <returns>The sensor rule detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(SensorRule))]
        public IHttpActionResult Get(int id)
        {
            var sensor = this.sensorRuleService.Get(id);

            if (sensor == null)
            {
                return this.NotFound();
            }

            return this.Ok(sensor);
        }

        /// <summary>
        /// Gets all sensor rules.
        /// </summary>
        /// <returns>The sensor rule details.</returns>
        public List<SensorRule> GetAll()
        {
            var sensorRules = this.sensorRuleService.GetAll();
            return sensorRules;
        }

        /// <summary>
        /// Creates the specified sensor rule.
        /// </summary>
        /// <param name="id">The sensor group identifier.</param>
        /// <param name="sensorRules">The sensor rule.</param>
        /// <returns>
        /// The created(201) on successfully creation else BadRequest(400) status code.
        /// </returns>
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Post(int id, List<SensorRule> sensorRules)
        {
            if (id < 1)
            {
                return this.BadRequest("Sensor group id must be grater than 0.");
            }

            if (sensorRules == null || sensorRules.Count < 1 || sensorRules.Any(s => s.CapabilityFilterId < 1))
            {
                return this.BadRequest("Invalid sensor rule model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.sensorRuleService.Create(id, sensorRules);

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
        /// Updates the specified sensor rule.
        /// Can not modify sensor group.
        /// </summary>
        /// <param name="sensorRule">The sensor rule.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Put(SensorRule sensorRule)
        {
            if (sensorRule == null || sensorRule.Id < 1)
            {
                return this.BadRequest("Invalid sensor rule model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.sensorRuleService.Update(sensorRule);

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
        /// Deletes the specified sensor rule.
        /// </summary>
        /// <param name="id">The sensor rule identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Sensor rule id must be grater than 0.");
            }

            var operationStatus = await this.sensorRuleService.Delete(id);

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
