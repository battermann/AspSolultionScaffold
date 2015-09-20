namespace ProjectScaffold.DomainModels

open System
open InternalMessageBus
open Chessie.ErrorHandling

exception AggregateNotFoundException
exception ConcurrencyFailureException
exception AggregateAlreadyExistsException

// ErrorMessages
type DomainMessage =
    | AggregateNotFound
    | DbUpdateError of string
    | SqlException of string
    | UnknownError of string
    | CouldNotLoadAggregate of string
    | ConcurrencyFailure
    | AggregateAlreadyExists

type AggregateId = AggregateId of string
    with
    static member Empty = AggregateId String.Empty

type AggregateVersion = AggregateVersion of int
    with
    static member Empty = (AggregateVersion -1)
    static member versionFrom =
        function 
        | AggregateVersion v -> v
    static member incrVersion version =
        AggregateVersion ((AggregateVersion.versionFrom version) + 1)