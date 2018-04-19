namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.API.HttpFilters;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services;

    /// <summary>
    /// Provides sensor APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class SensorController : ApiController
    {
        private readonly ISensorService sensorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorController"/> class.
        /// </summary>
        /// <param name="sensorService">The sensor service.</param>
        public SensorController(ISensorService sensorService)
        {
            this.sensorService = sensorService;
        }

        /// <summary>
        /// Gets all sensors.
        /// </summary>
        /// <returns>The sensor details.</returns>
        public List<Sensor> GetAll()
        {
            var sensors = this.sensorService.GetAll();
            return sensors;
        }

        /// <summary>
        /// Gets all unmapped sensors.
        /// </summary>
        /// <returns>The unmapped sensor details.</returns>
        [Route("GetAllUnmappedSensors")]
        public List<Sensor> GetAllUnmappedSensors()
        {
            var sensors = this.sensorService.GetAllUnmappedSensors();
            return sensors;
        }

        /// <summary>
        /// Gets all sensors by sensor type.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>
        /// The sensor details.
        /// </returns>
        [Route("GetAllSensorBySensorType/{sensorTypeId}")]
        [ResponseType(typeof(List<Sensor>))]
        public IHttpActionResult GetAllSensorBySensorType(int sensorTypeId)
        {
            if (sensorTypeId < 1)
            {
                return this.BadRequest("Sensor id must be grater than 0.");
            }

            var sensors = this.sensorService.GetAllSensorBySensorType(sensorTypeId);
            return this.Ok(sensors);
        }

        /// <summary>
        /// Gets the specified sensor.
        /// </summary>
        /// <param name="id">The sensor identifier.</param>
        /// <returns>The sensor detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(Sensor))]
        public IHttpActionResult Get(int id)
        {
            var sensor = this.sensorService.Get(id);

            if (sensor == null)
            {
                return this.NotFound();
            }

            return this.Ok(sensor);
        }

        /// <summary>
        /// Creates the specified sensor.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public IHttpActionResult Post(Sensor sensor)
        {
            if (sensor == null)
            {
                return this.BadRequest("Invalid sensor model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.sensorService.Create(sensor);

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
        /// Updates the specified sensor.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        /// <returns>The updated(204) on successfully updation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public IHttpActionResult Put(Sensor sensor)
        {
            if (sensor == null || sensor.Id < 1)
            {
                return this.BadRequest("Invalid sensor type model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.sensorService.Update(sensor);

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
        /// Deletes the specified sensor.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="id">The sensor identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [HttpDelete]
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Sensor id must be grater than 0.");
            }

            var operationStatus = await this.sensorService.Delete(id);

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
        /// Gets the sensor type for given sensor key.
        /// </summary>
        /// <param name="sensorKey">The sensor key.</param>
        /// <returns>The sensor type if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(string))]
        [HttpPost]
        [Route("GetSensorType")]
        public IHttpActionResult GetSensorType([FromBody]string sensorKey)
        {
            if (string.IsNullOrWhiteSpace(sensorKey))
            {
                return this.BadRequest("Invalid sensor key.");
            }

            var sensorType = this.sensorService.GetSensorType(sensorKey);

            if (sensorType == null)
            {
                return this.NotFound();
            }

            return this.Ok(sensorType);
        }

        /// <summary>
        /// Adds the rooms to building.
        /// It receive only multipart/form-data content-type.
        /// It only accept sensor details in CSV file format.
        /// Sample Format -> SensorName, SensorKey.
        /// SensorKey - it must be valid string, if already exist than it's ignored.
        /// SensorName - it's optional, if not specified than sensor name will same as sensor key.
        /// </summary>
        /// <param name="sensorTypeId">The sensor type identifier.</param>
        /// <returns>The sensor added to sensor type confirmation.</returns>
        [HttpPost]
        [Route("BulkSensorUpload/{sensorTypeId}")]
        public IHttpActionResult BulkSensorUpload(int sensorTypeId)
        {
            if (sensorTypeId < 1)
            {
                return this.BadRequest("SensorType id must be grater than 0.");
            }

            var sensors = this.GetSensors().Result;

            if (sensors.Count() == 0)
            {
                return this.BadRequest("Invalid sensor details.");
            }

            var operationStatus = this.sensorService.AddSensorToSensorType(sensorTypeId, sensors);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Ok)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return this.BadRequest(operationStatus.Message);
            }
        }

        private async Task<List<Sensor>> GetSensors()
        {
            // Check if the request contains multipart/form-data.
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var contents = await this.Request.Content.ReadAsMultipartAsync();

            var fileContent = contents.Contents.FirstOrDefault(c => c.Headers.ContentType != null && c.Headers.ContentType.MediaType.Equals("application/vnd.ms-excel") && c.Headers.ContentLength > 0);

            if (fileContent != null)
            {
                List<Sensor> sensors;
                using (var roomCSV = await fileContent.ReadAsStreamAsync())
                {
                    sensors = this.GetSensorFromCSV(roomCSV);
                }

                return sensors;
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        private List<Sensor> GetSensorFromCSV(Stream sensorCSV)
        {
            var sensors = new List<Sensor>();
            using (var reader = new StreamReader(sensorCSV))
            {
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(',');

                    if (values.Count() >= 1)
                    {
                        var sensorKey = values[0];
                        var sensorName = values.Count() > 1 ? values[1] : sensorKey;

                        if (!string.IsNullOrWhiteSpace(sensorKey))
                        {
                            sensors.Add(new Sensor
                            {
                                Name = sensorName,
                                SensorKey = sensorKey
                            });
                        }
                    }
                }
            }

            return sensors;
        }
    }
}
