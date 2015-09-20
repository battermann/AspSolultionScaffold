namespace ProjectScaffold.DomainModels

open System
open InternalMessageBus
open Chessie.ErrorHandling

// Item
type Item = { 
    id:AggregateId; 
    version: AggregateVersion;
    name:string; 
    description:string option }
    with
    static member initial =
        { id = AggregateId.Empty; version = AggregateVersion -1; name = ""; description = None }
    static member isEmpty item =
        item.version = AggregateVersion.Empty
    
    static member applyEvent itemResult event =
        let ap item =
            function 
            | ItemCreated data -> 
                match item.version = AggregateVersion.Empty with
                | true -> ok { item with id = data.id;  version = AggregateVersion 0; name = data.name; description = data.description }
                | _ -> fail DomainMessage.AggregateAlreadyExists
            | ItemUpdated data -> 
                match data.expectedVersion = AggregateVersion.incrVersion item.version with
                | true -> ok { item with version = AggregateVersion.incrVersion item.version; name = data.name; description = data.description }
                | _ -> fail DomainMessage.ConcurrencyFailure

        itemResult >>= fun item -> ap item event



