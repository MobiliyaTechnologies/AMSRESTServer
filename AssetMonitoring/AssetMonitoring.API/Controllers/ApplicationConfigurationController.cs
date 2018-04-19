namespace AssetMonitoring.API.Controllers
{
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
    /// Provides application configuration APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class ApplicationConfigurationController : ApiController
    {
        private readonly IBlobStorageService blobStorageService;
        private readonly IApplicationConfigurationService applicationConfigurationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigurationController" /> class.
        /// </summary>
        /// <param name="blobStorageService">The BLOB storage service.</param>
        /// <param name="applicationConfigurationService">The application configuration service.</param>
        public ApplicationConfigurationController(IBlobStorageService blobStorageService, IApplicationConfigurationService applicationConfigurationService)
        {
            this.blobStorageService = blobStorageService;
            this.applicationConfigurationService = applicationConfigurationService;
        }

        /// <summary>
        /// Adds the logo.
        /// It receive only multipart/form-data content-type.
        /// Application logo file with any name but must be image.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <returns>The logo uploaded status.</returns>
        [Route("ApplicationLogo")]
        [HttpPost]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> AddLogo()
        {
            var logo = await this.GetApplicationLogoFromRequest();

            if (logo == null)
            {
                return this.BadRequest("Invalid application logo.");
            }

            this.blobStorageService.UploadBlob(logo);
            return this.StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Gets the application logo URL.
        /// </summary>
        /// <returns>The application logo URL.</returns>
        [Route("ApplicationLogoUrl")]
        public string GetApplicationLogoUrl()
        {
            var logo = new BlobStorage()
            {
                BlobName = ApiConstant.LogoBlobName,
                StorageContainer = ApplicationConstant.BlobPublicContainer,
            };

            return this.blobStorageService.GetBlobUri(logo);
        }

        /// <summary>
        /// Gets all application configuration.
        /// </summary>
        /// <returns>The application configuration.</returns>
        [ResponseType(typeof(List<Configuration>))]
        public HttpResponseMessage GetAllApplicationConfiguration()
        {
            var appConfigs = this.applicationConfigurationService.GetAllApplicationConfiguration();
            return this.Request.CreateResponse(HttpStatusCode.OK, appConfigs);
        }

        /// <summary>
        /// Gets the application configuration for given configuration type.
        /// </summary>
        /// <param name="applicationConfigurationType">Type of the application configuration.</param>
        /// <returns>The application configuration if fount else a not found error response.</returns>
        [Route("ApplicationConfiguration/{applicationConfigurationType}")]
        [ResponseType(typeof(Configuration))]
        public IHttpActionResult GetApplicationConfiguration(string applicationConfigurationType)
        {
            var appConfig = this.applicationConfigurationService.GetApplicationConfiguration(applicationConfigurationType);

            if (appConfig != null)
            {
                return this.Ok(appConfig);
            }

            return this.NotFound();
        }

        /// <summary>
        /// Adds the application configuration.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="configuration">The application configuration model.</param>
        /// <returns>The created(201) on successfully creation else BadRequest(400) status code.</returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Post(Configuration configuration)
        {
            if (configuration == null)
            {
                return this.BadRequest("Invalid application configuration model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (configuration.ApplicationConfigurationEntries.Count == 0)
            {
                return this.BadRequest("Application configuration entries must required.");
            }

            var operationStatus = this.applicationConfigurationService.AddApplicationConfiguration(configuration);

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
        /// Deletes the application configuration for given configuration type.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="applicationConfigurationType">Type of the application configuration.</param>
        /// <returns>The deleted(204) on successfully deletion else BadRequest(400) status code.</returns>
        [Route("ApplicationConfiguration/{applicationConfigurationType}")]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Delete(string applicationConfigurationType)
        {
            if (string.IsNullOrWhiteSpace(applicationConfigurationType))
            {
                return this.BadRequest("Invalid application configuration type.");
            }

            var operationStatus = this.applicationConfigurationService.DeleteApplicationConfiguration(applicationConfigurationType);

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
        /// Updates the application configuration entry.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="applicationConfigurationEntryModel">The application configuration entry model.</param>
        /// <returns>The modification confirmation, or bad request error response if invalid input parameters.</returns>
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult Put(ApplicationConfigurationEntry applicationConfigurationEntryModel)
        {
            if (applicationConfigurationEntryModel == null || applicationConfigurationEntryModel.Id < 1)
            {
                return this.BadRequest("Invalid application configuration entry model.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var operationStatus = this.applicationConfigurationService.UpdateApplicationConfigurationEntry(applicationConfigurationEntryModel);

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
        /// Deletes the application configuration entry for given id.
        /// This API is accessible to only super admin user.
        /// </summary>
        /// <param name="applicationConfigurationEntryId">The application configuration entry identifier.</param>
        /// <returns>The deletion confirmation.</returns>
        [Route("ApplicationConfigurationEntry/{applicationConfigurationEntryId}")]
        [CustomAuthorize(UserRole = UserRole.SuperAdmin)]
        [OverrideAuthorization]
        [ResponseType(typeof(StatusCodeResult))]
        public IHttpActionResult DeleteApplicationConfigurationEntry(int applicationConfigurationEntryId)
        {
            if (applicationConfigurationEntryId < 1)
            {
                return this.BadRequest("Application configuration entry id must be grater than zero.");
            }

            var operationStatus = this.applicationConfigurationService.DeleteApplicationConfigurationEntry(applicationConfigurationEntryId);

            if (operationStatus.StatusCode == Contracts.Enums.StatusCode.Ok)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return this.BadRequest(operationStatus.Message);
            }
        }

        private async Task<BlobStorage> GetApplicationLogoFromRequest()
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
                var logoDetail = new BlobStorage
                {
                    BlobName = ApiConstant.LogoBlobName,
                    IsPublicContainer = true,
                    StorageContainer = ApplicationConstant.BlobPublicContainer,
                    Blob = await fileContent.ReadAsStreamAsync(),
                    BlobType = fileContent.Headers.ContentType.ToString()
                };
                return logoDetail;
            }
            else
            {
                return null;
            }
        }
    }
}
