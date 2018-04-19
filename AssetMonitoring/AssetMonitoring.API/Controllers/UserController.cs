namespace AssetMonitoring.API.Controllers
{
    using System.Web.Http;
    using AssetMonitoring.Components.Context;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Services;

    /// <summary>
    /// Provide user APIs.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class UserController : ApiController
    {
        private readonly IContextInfoProvider context;
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userService">The user service.</param>
        public UserController(IContextInfoProvider context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        /// <summary>
        /// Gets the Lodged-in user.
        /// </summary>
        /// <returns>The user detail.</returns>
        public User GetCurrentUser()
        {
            var user = this.userService.GetByB2cIdentifier(this.context.Current.B2cIdentifier);
            return user;
        }
    }
}
