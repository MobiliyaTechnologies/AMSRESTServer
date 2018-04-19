namespace AssetMonitoring.DocumentDB.WebJob.Function
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class FunctionInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().InNamespace("AssetMonitoring.DocumentDB.WebJob.Function")
                .LifestyleSingleton());
        }
    }
}
