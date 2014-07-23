using System.Web.Http;
using SingledOut.Data;
using SingledOut.Repository;
using SingledOut.Services.Interfaces;
using SingledOut.Services.Services;
using SingledOut.WebApi.Interfaces;
using SingledOut.WebApi.ModelFactory;
using WebApiContrib.IoC.Ninject;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SingledOut.WebApi.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(SingledOut.WebApi.App_Start.NinjectWebCommon), "Stop")]


namespace SingledOut.WebApi.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        
        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            // Register Dependencies
            RegisterServices(kernel);

            // Set Web API Resolver
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // Repositories.
            kernel.Bind<SingledOutContext>().To<SingledOutContext>().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();
            kernel.Bind<IQuestionRepository>().To<QuestionRepository>().InRequestScope();
            kernel.Bind<IAnswerRepository>().To<AnswerRepository>().InRequestScope();
            kernel.Bind<IUserQuestionRepository>().To<UserQuestionRepository>().InRequestScope();
            kernel.Bind<IUserAnswersRepository>().To<UserAnswersRepository>().InRequestScope();
            kernel.Bind<IUserLocationsRepository>().To<UserLocationsRepository>().InRequestScope();
            kernel.Bind<UserRepository>().To<UserRepository>().InRequestScope();

            // Querybuilders.
            kernel.Bind<Repository.QueryBuilders.User.IQueryBuilder>().To<Repository.QueryBuilders.User.QueryBuilder>().InRequestScope();
            
            // Model Factories.
            kernel.Bind<IUserModelFactory>().To<UserModelFactory>().InRequestScope();
            kernel.Bind<IUserQuestionModelFactory>().To<UserQuestionModelFactory>().InRequestScope();
            kernel.Bind<IUserAnswerModelFactory>().To<UserAnswerModelFactory>().InRequestScope();
            kernel.Bind<IUserLocationModelFactory>().To<UserLocationModelFactory>().InRequestScope();
            kernel.Bind<IAnswerModelFactory>().To<AnswerModelFactory>().InRequestScope();
            kernel.Bind<IQuestionModelFactory>().To<QuestionModelFactory>().InRequestScope();

            // Security.
            kernel.Bind<ISecurity>().To<Security>().InRequestScope();
            kernel.Bind<Security>().To<Security>().InRequestScope();
        }        
    }
}
