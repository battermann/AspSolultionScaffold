namespace ProjectScaffold.Data

module EventStore =
    open System
    open System.Data
    open System.Data.Linq
    open Microsoft.FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open Chessie.ErrorHandling
    open System.Net
    open Newtonsoft.Json
    open Microsoft.FSharp.Reflection
    open ProjectScaffold.DomainModels
    open Serialization

    [<Literal>]
    let connectionString = "Data Source=(localdb)\V11.0;Initial Catalog=BRAVE.EventStore;Integrated Security=SSPI;"
    type private dbSchema = SqlDataConnection<connectionString>

    let private toEventDto (streamId, version, typeName, eventData) =
        dbSchema.ServiceTypes.Event(StreamId = streamId, EventNumber = version, TypeName = typeName, CreatedDate = DateTime.Now, EventData = eventData)

    let private db (connectionString:string) = 
            let db = dbSchema.GetDataContext(connectionString)
            // Enable the logging of database activity to the console.
            db.DataContext.Log <- System.Console.Out
            db

    let private getVersion (db: dbSchema.ServiceTypes.SimpleDataContextTypes.BRAVE_EventStore) streamId =
        let ns = 
            db.Event 
            |> Seq.filter (fun dto -> dto.StreamId = streamId)
            |> Seq.map (fun dto -> dto.EventNumber)
        if ns |> Seq.isEmpty then -1 else ns |> Seq.max

    let getStreamIds prefix connectionsstring =
        let db = db connectionString
        try
            db.Event 
            |> Seq.map (fun dto -> dto.StreamId)
            |> Seq.filter (fun id -> id.StartsWith(prefix))
            |> Seq.distinct
            |> Seq.toList
            |> ok
        with
        | ex -> fail (DbQueryError ex.Message)

    let readFromStream connectionstring streamId =
        let db = db connectionString
        try
            let events = 
                db.Event 
                |> Seq.filter (fun dto -> dto.StreamId = streamId)

            let casted = 
                events
                |> Seq.sortBy (fun dto -> dto.EventNumber)
                |> Seq.map (fun dto -> dto.EventData)
                |> Seq.map deserialize<Event>
                |> Seq.cast
                |> Seq.toList
            let version = if events |> Seq.isEmpty then -1 else events |> Seq.map (fun dto -> dto.EventNumber) |> Seq.max
            (version, casted) |> ok
        with
        | ex -> fail (DbQueryError ex.Message)
        
    let appendToStream connectionString streamId expectedVersion newEvents =
        let db = db connectionString
        try
            match getVersion db streamId with
            | version when version = expectedVersion -> 
                newEvents 
                |> List.map serialize
                |> List.mapi (fun i (name, data) -> toEventDto (streamId, expectedVersion + (i + 1), name, data))
                |> db.Event.InsertAllOnSubmit
                |> ignore

                db.DataContext.SubmitChanges() |> ok
            | _ -> fail ConcurrencyFailure
        with
        | ex -> fail (DbUpdateError ex.Message)