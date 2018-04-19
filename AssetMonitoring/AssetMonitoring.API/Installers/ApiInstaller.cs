namespace AssetMonitoring.API.Installers
{
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using AssetMonitoring.API.Context;
    using AssetMonitoring.API.Factory;
    using AssetMonitoring.Components.Context;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class ApiInstaller : IWindsorInstaller
    {
        void IWindsorInstaller.Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromThisAssembly().BasedOn<IHttpController>().If(t => t.Name.EndsWith("Controller")).LifestyleTransient());

            container.Register(Component.For<IHttpControllerActivator>().ImplementedBy<WindsorHttpControllerActivator>().LifestyleSingleton());

            container.Register(Component.For<IContextInfoProvider>().ImplementedBy<WebContextInfoProvider>().LifestylePerThread());
        }
    }
}