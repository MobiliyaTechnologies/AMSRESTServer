namespace AssetMonitoring.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.API.Helpers;
    using AssetMonitoring.API.HttpFilters;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services;
    using AssetMonitoring.Utilities;

    /// <summary>
    /// Provides indoor layout APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class IndoorLayoutController : ApiController
    {
        private readonly IBlobStorageService blobStorageService;
        private readonly IIndoorLayoutService indoorLayoutService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndoorLayoutController" /> class.
        /// </summary>
        /// <param name="blobStorageService">The BLOB storage service.</param>
        /// <param name="indoorLayoutService">The indoor layout service.</param>
        public IndoorLayoutController(IBlobStorageService blobStorageService, IIndoorLayoutService indoorLayoutService)
        {
            this.blobStorageService = blobStorageService;
            this.indoorLayoutService = indoorLayoutService;
        }

        /// <summary>
        /// Adds the indoor layout.
        /// It receive only multipart/form-data content-type.
        /// Indoor layout file with any name but must be image.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <returns>The indoor layout uploaded status.</returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Post()
        {
            var layout = await this.GetImageFromRequest();

            if (layout == null)
            {
                return this.BadRequest("Invalid indoor layout.");
            }

            this.blobStorageService.UploadBlob(layout);

            var indoorLayout = new IndoorLayout
            {
                Name = layout.BlobName,
                FileName = layout.BlobName,
                Description = layout.BlobName
            };

            this.indoorLayoutService.Add(indoorLayout);
            return this.StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Get all indoor layout.
        /// </summary>
        /// <returns>The indoor layouts.</returns>
        public List<IndoorLayout> GetAllIndoorLayout()
        {
            var indoreLayouts = this.indoorLayoutService.GetAll();
            var containerUri = this.blobStorageService.GetContainerUri(ApplicationConstant.BlobPublicContainer);

            foreach (var indoreLayout in indoreLayouts)
            {
                indoreLayout.FileUrl = string.Format("{0}/{1}", containerUri, indoreLayout.FileName);
            }

            return indoreLayouts;
        }

        /// <summary>
        /// Get the specified indoor layout.
        /// </summary>
        /// <param name="id">The indoor layout identifier.</param>
        /// <returns>The indoor layout if found else NotFound(404) status code.</returns>
        [ResponseType(typeof(IndoorLayout))]
        public IHttpActionResult Get(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Indoor layout id must be grater than 0.");
            }

            var indoreLayout = this.indoorLayoutService.Get(id);

            if (indoreLayout == null)
            {
                return this.NotFound();
            }

            var containerUri = this.blobStorageService.GetContainerUri(ApplicationConstant.BlobPublicContainer);
            indoreLayout.FileUrl = string.Format("{0}/{1}", containerUri, indoreLayout.FileName);

            return this.Ok(indoreLayout);
        }

        /// <summary>
        /// Maps gateway with indoor layout.
        /// </summary>
        /// <param name="indoorLayout">The indoor layout.</param>
        /// <returns>The updated(204) on successfully mapping else BadRequest(400) status code.</returns>
        public IHttpActionResult Put(IndoorLayout indoorLayout)
        {
            var operationStatus = this.indoorLayoutService.MapGateway(indoorLayout);

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
        /// Detach gateway with indoor layout.
        /// </summary>
        /// <param name="indoorLayout">The indoor layout.</param>
        /// <returns>The updated(204) on successfully detaching gateway else BadRequest(400) status code.</returns>
        [Route("DetachGateway")]
        [HttpPut]
        public IHttpActionResult DetachGateway(IndoorLayout indoorLayout)
        {
            var operationStatus = this.indoorLayoutService.DetachGateway(indoorLayout);

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
        /// Deletes the specified indoor layout.
        /// </summary>
        /// <param name="id">The indoor layout identifier.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [ResponseType(typeof(StatusCodeResult))]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public IHttpActionResult Delete(int id)
        {
            if (id < 1)
            {
                return this.BadRequest("Indoor layout id must be grater than 0.");
            }

            var indoreLayout = this.indoorLayoutService.Get(id);

            if (indoreLayout == null)
            {
                return this.NotFound();
            }

            var operationStatus = this.indoorLayoutService.Delete(id);

            var layoutBlob = new BlobStorage
            {
                BlobName = indoreLayout.FileName,
                StorageContainer = ApplicationConstant.BlobPublicContainer,
            };

            this.blobStorageService.DeleteBlob(layoutBlob);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Ok)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return this.BadRequest(operationStatus.Message);
            }
        }

        private async Task<BlobStorage> GetImageFromRequest()
        {
            // Check if the request contains multipart/form-data.
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var contents = await this.Request.Content.ReadAsMultipartAsync();

            var fileContent = contents.Contents.FirstOrDefault(c => c.Headers.ContentType != null && c.Headers.ContentType.MediaType.StartsWith("image") && c.Headers.ContentLength > 0);

            if (fileContent != null)
            {
                var blob = new BlobStorage
                {
                    BlobName = ApiConstant.IndoorLayoutBlobName,//ApiConstant.IndoorLayoutBlobName + Guid.NewGuid(),
                    IsPublicContainer = true,
                    StorageContainer = ApplicationConstant.BlobPublicContainer,
                    Blob = await fileContent.ReadAsStreamAsync(),
                    BlobType = fileContent.Headers.ContentType.ToString()
                };
                return blob;
            }
            else
            {
                return null;
            }
        }
    }
}
