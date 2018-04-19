namespace AssetMonitoring.API
{
    using System.IdentityModel.Tokens;
    using AssetMonitoring.Utilities;
    using Microsoft.Owin.Security.Jwt;
    using Microsoft.Owin.Security.OAuth;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerAuthentication(this.CreateBearerOptionsFromPolicy(ApplicationConfiguration.B2cSignUpInPolicyId));
        }

        public OAuthBearerAuthenticationOptions CreateBearerOptionsFromPolicy(string policy)
        {
            TokenValidationParameters tvps = new TokenValidationParameters
            {
                // This is where you specify that your API only accepts tokens from its own clients
                ValidAudience = ApplicationConfiguration.B2cClientId,
                AuthenticationType = policy,
            };

            return new OAuthBearerAuthenticationOptions
            {
                // This SecurityTokenProvider fetches the Azure AD B2C meta-data & signing keys from the OpenIDConnect meta data endpoint
                AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(string.Format(ApplicationConfiguration.B2cAadInstance, ApplicationConfiguration.B2cTenant, policy))),
            };
        }
    }
}