namespace AssetMonitoring.DocumentDB.WebJob.Services
{
    using System.IO;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.StreamAnalytics.Services;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Microsoft.Azure.WebJobs.Host;

    public class JobActivator : IJobActivator
    {
        private readonly IWindsorContainer container;

        public JobActivator()
        {
            this.container = new WindsorContainer();
            this.InstallWindsorContainerInstallers();
        }

        public T CreateInstance<T>()
        {
            return this.container.Resolve<T>();
        }

        private void InstallWindsorContainerInstallers()
        {
            this.container.Register(Component.For<IWindsorContainer>().Instance(this.container).LifeStyle.Singleton);

            this.container.Install(FromAssembly.InDirectory(new AssemblyFilter(Directory.GetCurrentDirectory())));
        }
    }
}
