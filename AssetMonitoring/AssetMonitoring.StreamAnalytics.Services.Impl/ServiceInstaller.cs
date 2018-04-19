namespace AssetMonitoring.StreamAnalytics.Services.Impl
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().InNamespace("AssetMonitoring.StreamAnalytics.Services.Impl").If(t => t.Name.EndsWith("Service"))
                .WithService.DefaultInterfaces().LifestyleSingleton());
        }
    }
}
