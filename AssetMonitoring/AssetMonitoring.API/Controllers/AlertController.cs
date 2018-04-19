namespace AssetMonitoring.API.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.DocumentDbContract;
    using AssetMonitoring.Services;
    using AssetMonitoring.Utilities;

    /// <summary>
    /// Provides group alert APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api")]
    public class AlertController : ApiController
    {
        private readonly IAlertService alertService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertController"/> class.
        /// </summary>
        /// <param name="alertService">The alert service.</param>
        public AlertController(IAlertService alertService)
        {
            this.alertService = alertService;
        }

        /// <summary>
        /// Gets all alert by group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The alert details.</returns>
        [ResponseType(typeof(List<AlertDocument>))]
        [Route("GetAllAlertByGroup/{groupId}")]
        public IHttpActionResult GetAllAlertByGroup(int groupId)
        {
            if (groupId < 1)
            {
                return this.BadRequest("Group id must be grater than 0");
            }

            var alert = this.alertService.GetAlertByGroup(groupId);
            return this.Ok(alert);
        }

        /// <summary>
        /// Gets all paginating alert.
        /// </summary>
        /// <param name="alertPaginationFilter">The alert pagination filter.</param>
        /// <returns>
        /// The alert details.
        /// </returns>
        [HttpPost]
        [Route("PaginateAlert")]
        [ResponseType(typeof(List<AlertPaginationResult>))]
        public async Task<IHttpActionResult> GetPaginateAlert(AlertPaginationFilter alertPaginationFilter)
        {
            if (alertPaginationFilter == null)
            {
                return this.BadRequest("Alert pagination filter can not be null.");
            }

            if (alertPaginationFilter.GroupId < 1)
            {
                return this.BadRequest("Sensor group id must be grater than 0.");
            }

            if (!alertPaginationFilter.PageSize.HasValue)
            {
                alertPaginationFilter.PageSize = ApplicationConstant.AlertCount;
            }

            var assets = await this.alertService.GetPaginateAlert(alertPaginationFilter);

            if (assets == null)
            {
                return this.NotFound();
            }

            return this.Ok(assets);
        }

        /// <summary>
        /// Gets the group alert filter.
        /// </summary>
        /// <returns>The group alert filter lists.</returns>
        [Route("GroupAlertFilter")]
        public List<GroupAlertPaginateFilter> GetGroupAlertFilter()
        {
            var groupAleertFilter = this.alertService.GetGroupAlertFilter();
            return groupAleertFilter;
        }
    }
}
