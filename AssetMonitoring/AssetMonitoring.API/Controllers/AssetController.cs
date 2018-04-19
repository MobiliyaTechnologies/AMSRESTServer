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

    /// <summary>
    /// Provides asset APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class AssetController : ApiController
    {
        private readonly IAssetService assetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetController"/> class.
        /// </summary>
        /// <param name="assetService">The asset service.</param>
        public AssetController(IAssetService assetService)
        {
            this.assetService = assetService;
        }

        /// <summary>
        /// Gets all assets.
        /// </summary>
        /// <returns>The asset details.</returns>
        public List<GroupAsset> GetAll()
        {
            var assets = this.assetService.GetAll();
            return assets;
        }

        /// <summary>
        /// Gets all damage assets.
        /// </summary>
        /// <returns>The group asset details.</returns>
        [Route("DamageAsset")]
        public List<GroupAsset> GetAllDamageAsset()
        {
            var assets = this.assetService.GetAllDamagAsset();
            return assets;
        }

        /// <summary>
        /// Gets all damage assets by group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>
        /// The group asset details.
        /// </returns>
        [Route("DamageAsset/{gropId}")]
        public List<GroupAsset> GetAllDamageAsset(int groupId)
        {
            var assets = this.assetService.GetAllDamagAsset(groupId);
            return assets;
        }

        /// <summary>
        /// Gets the specified asset.
        /// </summary>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <returns>The asset detail if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(GroupAsset))]
        [Route("Asset/{assetBarcode}")]
        public IHttpActionResult Get(string assetBarcode)
        {
            if (string.IsNullOrWhiteSpace(assetBarcode))
            {
                return this.BadRequest("Invalid asset barcode.");
            }

            var asset = this.assetService.Get(assetBarcode);

            if (asset == null)
            {
                return this.NotFound();
            }

            return this.Ok(asset);
        }

        /// <summary>
        /// Creates the specified asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Post(Asset asset)
        {
            if (asset == null)
            {
                return this.BadRequest("Invalid asset model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.assetService.Create(asset);

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
        /// Deletes the specified asset.
        /// </summary>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <returns>
        /// The deleted(204) on successfully deletion else BadRequest(400) status code.
        /// </returns>
        [HttpDelete]
        [ResponseType(typeof(StatusCodeResult))]
        public async Task<IHttpActionResult> Delete([FromBody]string assetBarcode)
        {
            if (string.IsNullOrWhiteSpace(assetBarcode))
            {
                return this.BadRequest("Invalid asset barcode.");
            }

            var operationStatus = await this.assetService.Delete(assetBarcode);

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
        /// Gets the asset status.
        /// </summary>
        /// <param name="assetBarcode">The asset barcode.</param>
        /// <returns>The asset status.</returns>
        [HttpPost]
        [ResponseType(typeof(List<AssetStatus>))]
        [Route("AssetStatus")]
        public IHttpActionResult GetAssetStatus([FromBody] string assetBarcode)
        {
            if (string.IsNullOrWhiteSpace(assetBarcode))
            {
                return this.BadRequest("Invalid asset barcode.");
            }

            OperationStatus operationStatus = new OperationStatus();

            var assetStatus = this.assetService.GetAssetStatus(assetBarcode, operationStatus);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Error)
            {
                return this.BadRequest(operationStatus.Message);
            }
            else
            {
                return this.Ok(assetStatus);
            }
        }

        /// <summary>
        /// Detaches the asset sensor.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>The updated(204) on successfully detach else BadRequest(400) status code.</returns>
        [HttpPut]
        [ResponseType(typeof(StatusCodeResult))]
        [Route("DetachAssetSensor")]
        public async Task<IHttpActionResult> DetachAssetSensor(Asset asset)
        {
            if (asset == null)
            {
                return this.BadRequest("Invalid asset model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = await this.assetService.DetachSensor(asset);

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
