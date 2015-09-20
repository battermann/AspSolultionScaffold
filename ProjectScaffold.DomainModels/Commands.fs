namespace ProjectScaffold.DomainModels

open System
open InternalMessageBus
open Chessie.ErrorHandling

// Item
type ItemCommand =
| CreateItem of CreateItem
| UpdateItem of UpdateItem
and CreateItem = { timestamp:DateTime; id:AggregateId; name:string; description:string option } interface ICommand
and UpdateItem = { timestamp:DateTime; id:AggregateId; name:string; description:string option } interface ICommand


