namespace AssetMonitoring.API.Factory
{
    using System;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using Castle.MicroKernel;

    public class WindsorHttpControllerActivator : IHttpControllerActivator
    {
        private readonly IKernel windsorKernel;

        public WindsorHttpControllerActivator(IKernel kernel)
        {
            this.windsorKernel = kernel;
        }

        IHttpController IHttpControllerActivator.Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new System.Web.HttpException(404, "No Controller found for url" + request.RequestUri.ToString());
            }

            if (!this.windsorKernel.HasComponent(controllerType))
            {
                throw new InvalidOperationException("Controller type not registered with container:" + controllerType.FullName);
            }

            var httpController = (IHttpController)this.windsorKernel.Resolve(controllerType);
            return httpController;
        }
    }
}