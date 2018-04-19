namespace AssetMonitoring.API.HttpFilters
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using AssetMonitoring.API.Helpers;
    using AssetMonitoring.Components.Transaction;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services;
    using Castle.Windsor;

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public UserRole UserRole;

        public bool AllowMultiple { get { return true; } }

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
             base.OnAuthorization(actionContext);

            if (!actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                  && !actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() && actionContext.Response == null)
            {
                var userModel = await this.InitializaUser();
                userModel.Authorization = actionContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                //if (this.UserRole != UserRole.Any && this.UserRole != userModel.Role)
                //{
                //    actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User does not have a permission to perform this operation");
                //}
                //else
                //{
                // set user context
                var request = (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];
                request.Properties.Add("Context", userModel);
                //}
            }
        }

        private async Task<User> InitializaUser()
        {
            IUserService userService = null;
            ITransaction transaction = null;
            User user = null;
            try
            {
                userService = WebApiApplication.Container.Resolve<IUserService>();

                transaction = WebApiApplication.Container.Resolve<ITransaction>();
                transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                user = userService.GetByB2cIdentifier(this.GetClaimValue(ApiConstant.B2cClaimObjectIdentifier));

                if (user == null)
                {
                    user = new User
                    {
                        B2cIdentifier = this.GetClaimValue(ApiConstant.B2cClaimObjectIdentifier),
                        Name = this.GetClaimValue(ApiConstant.B2cClaimFirstName),
                        Email = this.GetClaimValue(ApiConstant.B2cClaimEmail),
                    };
                    user.Id = await userService.Create(user);
                }

                transaction.CommitTransaction();
            }
            finally
            {
                if (userService != null)
                {
                    WebApiApplication.Container.Release(userService);
                }
                if (transaction != null)
                {
                    WebApiApplication.Container.Release(transaction);
                }
            }

            return user;
        }

        private string GetClaimValue(string claimType)
        {
            var claim = ClaimsPrincipal.Current.FindFirst(claimType);
            return claim != null ? claim.Value : string.Empty;
        }
    }
}