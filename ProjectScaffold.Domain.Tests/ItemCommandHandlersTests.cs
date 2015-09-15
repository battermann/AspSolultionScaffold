using System;
using FSharpx;
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
        public void Test()
        {
            var sut = new ItemCommandHandlers();

            var id = Guid.NewGuid().ToString();
            var now = new DateTime(2015, 1, 1);

            var cmd = new CreateItem(
                timestamp: now,
                id: ItemId.NewItemId(id),
                name: "name",
                description: "description".Some());

            var result = sut.Handle(cmd);

            Check.That(result).ContainsExactly(new ItemCreated(now, ItemId.NewItemId(id), "name", "description".Some()));
        }
    }
}
