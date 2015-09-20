using System;
using System.Collections.Generic;
using Chessie.ErrorHandling.CSharp;
using InternalMessageBus;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Domain.CommandHandlers
{
    public class ItemCommandHandlers
    {
        private readonly IItemReadAccess _repository;

        public ItemCommandHandlers(IItemReadAccess repository)
        {
            _repository = repository;
        }

        public List<IEvent> Handle(CreateItem cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");

            return _repository.GetById(cmd.id)
                .Either(
                    ifSuccess: (item, _) => { throw new AggregateAlreadyExistsException(); },
                    ifFailure: _ => new List<IEvent>
                    {
                        // Todo: only if aggregate not found    
                        new ItemCreated(
                            timestamp: cmd.timestamp,
                            id: cmd.id,
                            expectedVersion: AggregateVersion.NewAggregateVersion(0),
                            name: cmd.name,
                            description: cmd.description)
                    });
        }

        public List<IEvent> Handle(UpdateItem cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");

            return _repository.GetById(cmd.id)
                .Either(
                    ifSuccess: (item, _) => new List<IEvent>
                    {
                        new ItemUpdated(
                            timestamp: cmd.timestamp,
                            expectedVersion: AggregateVersion.NewAggregateVersion(item.version.Item + 1),
                            id: cmd.id,
                            name: cmd.name,
                            description: cmd.description)
                    },
                    ifFailure: errs => { throw new AggregateNotFoundException(); });
        }
    }
}
