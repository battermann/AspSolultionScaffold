namespace ProjectScaffold.Domain

open ProjectScaffold.DomainModels
open System.Runtime.CompilerServices
open System

[<Extension>]
type IssueCSharpCompat =
    [<Extension>]
    static member Match<'T>(issue, (onInitial:Func<'T>), (onInProgress:Func<_,'T>)) =
        match issue with
        | InProgress s -> onInProgress.Invoke(s)
        | Issue.Initial -> onInitial.Invoke()

[<Extension>]
type DomainMessagesCSharpCompat =
    [<Extension>]
    static member Match(msg,
                        (onInvalidStateTransition: Func<string,_>),
                        (onDbUpdateError: Func<string,_>),
                        (onDbQueryError: Func<string,_>),
                        (onConcurrencyFailure: Func<_>),
                        (onAggregateNotFound: Func<_>),
                        (onCouldNotLoadAggregate: Func<string,_>),
                        (onAggregateAlreadyExists: Func<_>),
                        (onInvalidOperation: Func<string,_>),
                        (onUnknownError: Func<string,_>)) =
        match msg with
        | InvalidStateTransition s -> onInvalidStateTransition.Invoke(s)
        | DbUpdateError s -> onDbUpdateError.Invoke(s)
        | DbQueryError s -> onDbQueryError.Invoke(s)
        | ConcurrencyFailure -> onConcurrencyFailure.Invoke()
        | AggregateNotFound -> onAggregateNotFound.Invoke()
        | CouldNotLoadAggregate s -> onCouldNotLoadAggregate.Invoke(s)
        | AggregateAlreadyExists -> onAggregateAlreadyExists.Invoke()
        | InvalidOperation s -> onInvalidOperation.Invoke(s)
        | UnknownError s -> onUnknownError.Invoke(s)
        

[<Extension>]
type FSharpFuncExtensions =

    /// Convert an Action into an F# function returning unit
    [<Extension>]
    static member FromAction (f: Action) =
        fun () -> f.Invoke()
    
    /// Convert an Action into an F# function returning unit
    [<Extension>]
    static member FromAction (f: Action<_>) =
        fun x -> f.Invoke x

    /// Convert an Action into an F# function returning unit
    [<Extension>]
    static member FromAction (f: Action<_,_>) =
        fun x y -> f.Invoke(x,y)

    /// Convert an Action into an F# function returning unit
    [<Extension>]
    static member FromAction (f: Action<_,_,_>) =
        fun x y z -> f.Invoke(x,y,z)

    /// Convert a Func into an F# function
    [<Extension>]
    static member FromFunc (f: Func<_>) =
        fun () -> f.Invoke()

    /// Convert a Func into an F# function
    [<Extension>]
    static member FromFunc (f: Func<_,_>) =
        fun x -> f.Invoke x

    /// Convert a Func into an F# function
    [<Extension>]
    static member FromFunc (f: Func<_,_,_>) =
        fun x y -> f.Invoke(x,y)

    /// Convert a Func into an F# function
    [<Extension>]
    static member FromFunc (f: Func<_,_,_,_>) =
        fun x y z -> f.Invoke(x,y,z)

    /// Convert a Func into an F# function
    [<Extension>]
    static member FromFunc (f: Func<_,_,_,_,_>) =
        fun x1 x2 x3 x4-> f.Invoke(x1,x2,x3,x4)

