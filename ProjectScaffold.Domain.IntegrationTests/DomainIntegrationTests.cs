using System;
using Chessie.ErrorHandling.CSharp;
using InternalMessageBus;
using Microsoft.FSharp.Core;
using NFluent;
using NUnit.Framework;
using ProjectScaffold.Data;
using ProjectScaffold.Domain.CommandHandlers;
using ProjectScaffold.Domain.EventHandlers;
using ProjectScaffold.DomainModels;
using FSharpx;

namespace ProjectScaffold.Domain.IntegrationTests
{
    [TestFixture, Category("IntegrationText"), Explicit]
    public class DomainIntegrationTests
    {
        private ICommandSender _sender;
        private IItemReadAccess _ra;

        [SetUp]
        public void SetUp()
        {
            var repository = new AccessLayer.ItemRepository(@"Data Source=(localdb)\V11.0;Initial Catalog=ProjectScaffold.Database;Integrated Security=SSPI;");
            _ra = repository;
            IItemWriteAccess wa = repository;
            var commandHandlers = new ItemCommandHandlers();
            var eventHandlers = new ItemEventHandlers(wa);
            var bus = new MessageBus();
            ISubscribable subscribable = bus;
            _sender = bus;
            subscribable.RegisterCommandHandler<CreateItem>(commandHandlers.Handle);
            subscribable.RegisterCommandHandler<UpdateItem>(commandHandlers.Handle);
            subscribable.RegisterEventHandler<ItemCreated>(eventHandlers.Handle);
            subscribable.RegisterEventHandler<ItemUpdated>(eventHandlers.Handle);
        }


        [Test]
        public void InsertItem_Test()
        {
            var id = Guid.NewGuid().ToString();

            var cmd = new CreateItem(
                timestamp: DateTime.Now,
                id: ItemId.NewItemId(id), 
                name: "item 1",
                description: "This is item 1".Some());

            _sender.Send(cmd);

            var result = _ra.GetAll();

            result.Match(
                ifSuccess: (items, _) => Check.That(items).Contains(new Item(ItemId.NewItemId(id), "item 1", "This is item 1".Some())),
                ifFailure: errs => Assert.Fail());
        }

        [Test]
        public void Insert_and_update_item_Test()
        {
            var id = Guid.NewGuid().ToString();

            var insertCmd = new CreateItem(
                timestamp: DateTime.Now,
                id: ItemId.NewItemId(id),
                name: "item x",
                description: "This is item x".Some());

            _sender.Send(insertCmd);

            var updateCmd = new UpdateItem(
                timestamp: DateTime.Now,
                id: ItemId.NewItemId(id),
                name: "updated name",
                description: FSharpOption<string>.None);

            _sender.Send(updateCmd);

            var result = _ra.GetById(ItemId.NewItemId(id));

            result.Match(
                ifSuccess: (item, _) => Check.That(item).IsEqualTo(new Item(ItemId.NewItemId(id), "updated name", FSharpOption<string>.None)),
                ifFailure: errs => Assert.Fail());
        }
    }
}
