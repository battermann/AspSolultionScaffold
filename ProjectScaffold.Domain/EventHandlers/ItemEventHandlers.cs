using System;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Domain.EventHandlers
{
    public class ItemEventHandlers
    {
        private readonly IItemWriteAccess _repositor;

        public ItemEventHandlers(IItemWriteAccess repositor)
        {
            _repositor = repositor;
        }

        public void Handle(ItemCreated msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");

            var @event = ItemEvent.NewItemCreated(msg);

            _repositor.Update(@event);
        }

        public void Handle(ItemUpdated msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");

            var @event = ItemEvent.NewItemUpdated(msg);

            _repositor.Update(@event);
        }
    }
}
