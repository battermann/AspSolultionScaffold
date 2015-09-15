using System;
using System.Collections.Generic;
using InternalMessageBus;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Domain.CommandHandlers
{
    public class ItemCommandHandlers
    {
        public List<IEvent> Handle(CreateItem cmd)
        {
            if(cmd == null) throw new ArgumentNullException("cmd");

            return new List<IEvent>
            {
                new ItemCreated(
                    timestamp: cmd.timestamp,
                    id: cmd.id,
                    name: cmd.name,
                    description: cmd.description)
            };
        }

        public List<IEvent> Handle(UpdateItem cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");

            return new List<IEvent>
            {
                new ItemUpdated(
                    timestamp: cmd.timestamp,
                    id: cmd.id,
                    name: cmd.name,
                    description: cmd.description)
            };
        }
    }
}
