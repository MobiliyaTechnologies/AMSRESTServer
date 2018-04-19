namespace AssetMonitoring.Components.Transaction.Impl
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    /// <summary>
    /// Handles registration of <see cref="ITransaction"/> with Windsor's IOC container as per-web-request.
    /// </summary>
    public sealed class TransactionInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component
                .For<ITransaction>()
                .ImplementedBy<Transaction>()
            );
        }
    }
}
