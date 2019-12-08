using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.AzureServices;
using Apsis.Business;
using Apsis.Notification;
using Apsis.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace Apsis.DependencyResolver
{
    /// <summary>
    /// Configuration of SimpleInjector
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="container"></param>
        public static void IntegrateSimpleInjector(IServiceCollection services, Container container)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
            services.EnableSimpleInjectorCrossWiring(container);
            services.UseSimpleInjectorAspNetRequestScoping(container);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public static void InitilizeContainer(IApplicationBuilder app, Container container)
        {
            // Business
            container.Register<IRequestManager, RequestManager>();
            container.Register<IFileOperationsManager, FileOperationsManager>();
            container.Register<ICourseManager, CourseManager>();
            container.Register<IUserManager, UserManager>();
            container.Register<IUploadManager, UploadManager>();
            container.Register<IUploadHelper, UploadHelper>();
            container.Register<INotificationManager, NotificationManager>();
            // Repository
            container.Register<IRepository, SqlBaseRepository>();
            container.Register<IRequestRepository, RequestRepository>();
            container.Register<ICourseRepository, CourseRepository>();
            container.Register<IUserRepository, UserRepository>();
            container.Register<IUploadRepository, UploadRepository>();
            // Azure services
            container.Register<IBlobHelper, BlobHelper>();

            // Notification services
            container.Register<IEmailHelper, EmailHelper>();
            container.Register<IEmailManager, EmailManager>();

            container.AutoCrossWireAspNetComponents(app);
            container.Verify();
        }
    }
}
