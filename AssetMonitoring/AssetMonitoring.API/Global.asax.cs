namespace AssetMonitoring.API
{
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Mvc;
    using AssetMonitoring.API.ErrorHandler;
    using AssetMonitoring.API.HttpFilters;
    using AssetMonitoring.Utilities;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Microsoft.ApplicationInsights;

    public class WebApiApplication : System.Web.HttpApplication
    {
        internal static readonly IWindsorContainer Container = new WindsorContainer();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            this.RegisterAPIFilters(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configure(WebApiConfig.Register);

            Container.Kernel.ComponentModelCreated += new ComponentModelDelegate(this.Kernel_ComponentModelCreated);
            this.InstallWindsorContainerInstallers();
            this.SetControllerFactory(Container);

            // set application insights instrumentation key.
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = ApplicationConfiguration.ApplicationInsightsInstrumentationKey;
        }

        private void RegisterAPIFilters(HttpConfiguration config)
        {
            config.Filters.Add(new UnitOfWorkHttpFilterAttribute(Container));
            config.Filters.Add(new CustomAuthorizeAttribute());

            config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger(new TelemetryClient()));
        }

        private void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
        {
            if (model.LifestyleType == Castle.Core.LifestyleType.Undefined)
            {
                model.LifestyleType = Castle.Core.LifestyleType.PerWebRequest;
            }
        }

        private void SetControllerFactory(IWindsorContainer container)
        {
            var httpControllerFactory = container.Resolve<IHttpControllerActivator>();

            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator),
                httpControllerFactory);
        }

        private void InstallWindsorContainerInstallers()
        {
            Container.Register(Component.For<IWindsorContainer>().Instance(Container).LifeStyle.Singleton);

            Container.Install(FromAssembly.InDirectory(new AssemblyFilter(HttpRuntime.BinDirectory, "*.dll")));
        }
    }
}
