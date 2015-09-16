namespace ProjectScaffold.DomainModels

open System
open InternalMessageBus
open Chessie.ErrorHandling

// ErrorMessages
type DomainMessage =
    | ItemNotFound
    | DbUpdateError of string
    | SqlException of string
    | UnknownError of string

// Aggregates
type ItemId = ItemId of string
type Item = { id:ItemId; name:string; description:string option }

// Events
type ItemEvent =
| ItemCreated of ItemCreated
| ItemUpdated of ItemUpdated
and ItemCreated = { timestamp:DateTime; id:ItemId; name:string; description:string option } interface IEvent
and ItemUpdated = { timestamp:DateTime; id:ItemId; name:string; description:string option } interface IEvent

// Commands
type ItemCommand =
| CreateItem of CreateItem
| UpdateItem of UpdateItem
and CreateItem = { timestamp:DateTime; id:ItemId; name:string; description:string option } interface ICommand
and UpdateItem = { timestamp:DateTime; id:ItemId; name:string; description:string option } interface ICommand

// DAL interfaces
type IItemReadAccess =
    /// Return all Items
    abstract GetAll : unit -> Result<Item seq, DomainMessage>

    /// Return the customer with the given ItemId, or ItemNotFound error if not found
    abstract GetById : ItemId -> Result<Item, DomainMessage>

type IItemWriteAccess =
    abstract Update : ItemEvent -> Result<unit, DomainMessage>


