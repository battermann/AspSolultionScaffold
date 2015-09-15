using System.Web.Mvc;
using InternalMessageBus;
using ProjectScaffold.Domain.CommandHandlers;
using ProjectScaffold.Domain.EventHandlers;
using ProjectScaffold.DomainModels;

namespace WebProjectScaffold.Web
{
    public class MessageBusFactory : AbstractMessageBusFactory
    {
        protected override MessageBus Create()
        {
            var bus = new MessageBus();

            // register command handlers
            bus.RegisterCommandHandler<CreateItem>(x => DependencyResolver.Current.GetService<ItemCommandHandlers>().Handle(x));
            bus.RegisterCommandHandler<UpdateItem>(x => DependencyResolver.Current.GetService<ItemCommandHandlers>().Handle(x));

            // register event handlers
            bus.RegisterEventHandler<ItemCreated>(x => DependencyResolver.Current.GetService<ItemEventHandlers>().Handle(x));
            bus.RegisterEventHandler<ItemUpdated>(x => DependencyResolver.Current.GetService<ItemEventHandlers>().Handle(x));

            return bus;
        }
    }
}