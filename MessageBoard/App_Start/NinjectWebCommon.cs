[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MessageBoard.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MessageBoard.App_Start.NinjectWebCommon), "Stop")]

namespace MessageBoard.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Services;
    using Data;
    using System.Web.Http;
    using WebApiContrib.IoC.Ninject;

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
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            #if DEBUG
                  kernel.Bind<IMailService>().To<MockMailService>().InRequestScope();
#else
                  kernel.Bind<IMailService>().To<MailService>().InRequestScope();
#endif
            
            kernel.Bind<MessageBoardContext>().To<MessageBoardContext>().InRequestScope();//When any object needs MessageBoardContext, then give them MessageBoardContext

            /* kernel.Bind<IMessageBoardRepository>().To<MessageBoardRepository>().InRequestScope() ;
             * When an object request the IMessageBoardRepository DI (Dependecy Injection) will lookup the class that's associated with it
               and return an instance of the MEssageBoardRepository object.
               Since the MessageBoardRepository object has a parameter, it will then request the object (MessageBoardRepository).
               kernel.Bind<MessageBoardContext>().To<MessageBoardContext>().InRequestScope() - this will be called and return an instance
               of the MessageBoardContext object to the MessageBoardRepository original request. 
               InRequestScope() ensures that the requestor only works the same copy of the return instance. Prevents having to recreate
               the request object again. Why would you use InRequestScope() - is to make sure that a single instance of an object
               is shared by all objects created via the Ninject kernel for that HTTP request.
             */
            kernel.Bind<IMessageBoardRepository>().To<MessageBoardRepository>().InRequestScope() ;
        }
    }
}
