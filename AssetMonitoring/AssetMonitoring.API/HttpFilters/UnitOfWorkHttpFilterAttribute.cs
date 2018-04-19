namespace AssetMonitoring.API.HttpFilters
{
    using System;
    using AssetMonitoring.Components.Transaction;
    using Castle.Windsor;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class UnitOfWorkHttpFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private readonly IWindsorContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkHttpFilterAttribute" /> class.
        /// </summary>
        /// <param name="container">The IoC container.</param>
        public UnitOfWorkHttpFilterAttribute(IWindsorContainer container)
        {
            this.container = container;
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // these checks are required for the code contracts
            if (actionContext == null)
            {
                return;
            }

            System.Diagnostics.Trace.WriteLine("UnitOfWorkAttribute: Starting.");

            var transaction = this.container.Resolve<ITransaction>();
            try
            {
                transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            }
            finally
            {
                this.container.Release(transaction);
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            // these checks are required for the code contracts
            if (actionExecutedContext == null)
            {
                return;
            }

            System.Diagnostics.Trace.WriteLine("UnitOfWorkAttribute: Ending.");

            var transaction = this.container.Resolve<ITransaction>();
            try
            {
                if (actionExecutedContext.Exception == null && actionExecutedContext.Response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    transaction.CommitTransaction();
                }
                else
                {
                    try
                    {
                        transaction.RollbackTransaction();
                    }
                    catch (Exception)
                    {
                        // rolling back the transaction sometimes causes an exception to be raised
                        // throwing another exception causes the first exception to be lost
                        // logging this exception is difficult, due to the location of this filter
                        // also, the details of this exception are that the rollback failed (not too interesting)
                    }
                }
            }
            finally
            {
                this.container.Release(transaction);
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}