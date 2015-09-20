namespace ProjectScaffold.DomainModels

open System
open InternalMessageBus
open Chessie.ErrorHandling

// DAL interfaces
type IItemReadAccess =
    /// Return all Items
    abstract GetAll : unit -> Result<Item seq, DomainMessage>

    /// Return the customer with the given ItemId, or ItemNotFound error if not found
    abstract GetById : AggregateId -> Result<Item, DomainMessage>

type IItemWriteAccess =
    abstract Update : ItemEvent -> Result<unit, DomainMessage>

type IItemsEventStore =
    /// Return all ItemEvents
    abstract GetAll : unit -> Result<ItemEvent seq, DomainMessage>

    /// Get all events by aggregate id
    abstract GetById : AggregateId -> Result<ItemEvent seq, DomainMessage>

    /// Record event
    abstract Record : ItemEvent -> Result<unit, DomainMessage>
