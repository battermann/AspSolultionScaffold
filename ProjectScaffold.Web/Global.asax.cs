using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using InternalMessageBus;
using ProjectScaffold.Data;
using ProjectScaffold.Domain.CommandHandlers;
using ProjectScaffold.Domain.EventHandlers;
using ProjectScaffold.DomainModels;

namespace WebProjectScaffold.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ConfigureIoc();
        }

        private static void ConfigureIoc()
        {
            var settings = ConfigurationManager.ConnectionStrings;
            var connectionString = settings["DbConnection"].ConnectionString;
            var eventStoreConnectionString = settings["SqlEventStore"].ConnectionString;

            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // dal
            builder.RegisterType<EventStore.ItemStore>()
                .As<IItemsEventStore>()
                .WithParameter("connectionString", eventStoreConnectionString)
                .InstancePerRequest();

            builder.RegisterType<Repository.ItemRepository>()
                .As<IItemReadAccess>()
                .As<IItemWriteAccess>()
                .InstancePerRequest();

            // event handlers
            builder.RegisterType<ItemEventHandlers>()
                .InstancePerRequest();

            // command handlers
            builder.RegisterType<ItemCommandHandlers>()
                .InstancePerRequest();

            // message bus
            builder.RegisterType<MessageBusFactory>()
                .As<ICommandSenderFactory>()
                .SingleInstance();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
