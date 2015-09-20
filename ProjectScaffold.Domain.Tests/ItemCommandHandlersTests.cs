using System;
using Chessie.ErrorHandling;
using FSharpx;
using Moq;
using NFluent;
using NUnit.Framework;
using ProjectScaffold.Domain.CommandHandlers;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Domain.Tests
{
    [TestFixture]
    public class ItemCommandHandlersTests
    {
        [Test]
        public void HandleCreateItem()
        {
            //var id = Guid.NewGuid().ToString();

            //var sut = new ItemCommandHandlers(Mock.Of<IItemReadAccess>(x => x.GetById(AggregateId.NewAggregateId(id)) == Result<Item, DomainMessage>.FailWith(DomainMessage.AggregateNotFound)));

            //var now = new DateTime(2015, 1, 1);

            //var cmd = new CreateItem(
            //    timestamp: now,
            //    id: AggregateId.NewAggregateId(id),
            //    name: "name",
            //    description: "description".Some());

            //var result = sut.Handle(cmd);

            //Check.That(result).ContainsExactly(new ItemCreated(now, AggregateId.NewAggregateId(id), AggregateVersion.NewAggregateVersion(0), "name", "description".Some()));
        }
    }
}
