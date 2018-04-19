namespace AssetMonitoring.API.Context
{
    using System.Net.Http;
    using System.Web;
    using AssetMonitoring.Components.Context;
    using AssetMonitoring.Contracts;

    public class WebContextInfoProvider : IContextInfoProvider
    {
        UserContext IContextInfoProvider.Current
        {
            get
            {
                var request = (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];

                if (!request.Properties.ContainsKey("Context"))
                {
                    return null;
                }

                var user = (User)request.Properties["Context"];

                var userContext = new UserContext
                {
                    B2cIdentifier = user.B2cIdentifier,
                    Name = user.Name,
                    UserId = user.Id,
                    RoleId = user.RoleId,
                    RoleName = user.Role.ToString(),
                    Authorization = user.Authorization
                };

                return userContext;
            }
        }
    }
}