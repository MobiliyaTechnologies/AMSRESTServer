namespace AssetMonitoring.Components.Repository.EntityFramework
{
    using System.Data.Entity;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public sealed class RepositoryInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component
                .For<DbContext>()
                .UsingFactoryMethod(() =>
                {
                    return new EFDbContext();
                })
                .LifeStyle.PerWebRequest
            );

            container.Register(Component
                .For<IRepository>()
                .ImplementedBy<DbContextRepository>()
            );
        }

    }
}
