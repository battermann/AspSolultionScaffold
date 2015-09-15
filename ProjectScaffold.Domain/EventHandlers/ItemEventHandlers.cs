using System;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Domain.EventHandlers
{
    public class ItemEventHandlers
    {
        private readonly IItemWriteAccess _repository;

        public ItemEventHandlers(IItemWriteAccess repository)
        {
            _repository = repository;
        }

        public void Handle(ItemCreated msg)
        {
            if(msg == null) throw new ArgumentNullException("msg");

            _repository.Update(ItemEvent.NewItemCreated(msg));
        }

        public void Handle(ItemUpdated msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");

            _repository.Update(ItemEvent.NewItemUpdated(msg));
        }
    }
}
