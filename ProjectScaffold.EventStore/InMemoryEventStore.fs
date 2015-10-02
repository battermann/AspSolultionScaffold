namespace ProjectScaffold.Data

module InMemoryEventStore =
    open ProjectScaffold.DomainModels
    open Chessie.ErrorHandling

    type EventStream = {mutable Events: (Event * int) list} with
        member this.addEvents events = (this.Events <- events) |> ignore
        static member Version stream =
            stream.Events |> List.map snd |> List.max

    type EventStore = {mutable Streams : Map<string, EventStream> }

    let create() = {Streams = Map.empty }

    let getStreamWithVersion store streamId expectedVersion = 
        match store.Streams.TryFind streamId with
            | Some s when EventStream.Version s = expectedVersion -> s |> ok
            | None when expectedVersion = -1 -> 
                let s = {Events = List.empty}
                store.Streams <- store.Streams.Add(streamId, s)
                s |> ok
            | _ -> fail ConcurrencyFailure

    let appendToStream store streamId expectedVersion newEvents = 
        let stream = getStreamWithVersion store streamId expectedVersion
        let eventsWithVersion = newEvents |> List.mapi (fun index event -> (event, expectedVersion + index + 1)) 
        fun s -> (List.append s.Events eventsWithVersion) |> s.addEvents
        <!> stream

    let readFromStream store streamId =
        match store.Streams.TryFind streamId with
        | Some s -> s.Events |> List.fold (fun (maxV, es) (e, v) -> (v, es @ [e])) (0, []) |> ok
        | None -> (-1, []) |> ok