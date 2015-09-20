namespace ProjectScaffold.DomainModels

open System
open InternalMessageBus
open Chessie.ErrorHandling

// Item
type ItemEvent =
| ItemCreated of ItemCreated
| ItemUpdated of ItemUpdated
and ItemCreated = { timestamp:DateTime; id:AggregateId; expectedVersion:AggregateVersion; name:string; description:string option } interface IEvent
and ItemUpdated = { timestamp:DateTime; id:AggregateId; expectedVersion:AggregateVersion; name:string; description:string option } interface IEvent
      
module Events =
    let versionFrom =
        function
        | ItemCreated data -> data.expectedVersion
        | ItemUpdated data -> data.expectedVersion

    let idFrom =
        function
        | ItemCreated data -> data.id
        | ItemUpdated data -> data.id        