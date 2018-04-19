namespace AssetMonitoring.DocumentDB.WebJob.Logger
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class LoggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().InNamespace("AssetMonitoring.DocumentDB.WebJob.Logger")
                .LifestyleSingleton());
        }
    }
}
