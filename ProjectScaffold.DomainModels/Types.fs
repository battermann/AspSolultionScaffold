namespace ProjectScaffold.DomainModels

open System
open Chessie.ErrorHandling

type DomainMessage =
    // state transition messages
    | InvalidStateTransition of string

    // db messages
    | DbUpdateError of string
    | DbQueryError of string
    | ConcurrencyFailure

    // aggregate messages
    | AggregateNotFound
    | CouldNotLoadAggregate of string
    | AggregateAlreadyExists
    
    // broad messages
    | InvalidOperation of string
    | UnknownError of string

type UserName = UserName of string

type User = { Name:UserName }

type IssueId = IssueId of Guid with
    member this.Value =
        match this with
        | IssueId id -> id
    static member Prefix = "Issue-"
