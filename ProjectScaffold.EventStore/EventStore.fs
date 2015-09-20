namespace ProjectScaffold.Data

module EventStore =
    open System
    open System.Data
    open System.Data.Linq
    open Microsoft.FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open Serialization
    open ProjectScaffold.DomainModels
    open Chessie.ErrorHandling

    [<Literal>]
    let connectionString = "Data Source=(localdb)\V11.0;Initial Catalog=ProjectScaffold.EventStore.Database;Integrated Security=SSPI;"
    type dbSchema = SqlDataConnection<connectionString>

    let private serialize toEventDto (event, AggregateId streamId, AggregateVersion version) = 
        let typeName, data = serializeUnion event
        let eventData = System.Text.Encoding.UTF8.GetString(data)
        toEventDto(streamId, version, typeName, eventData)

    type ItemStore(connectionString:string) =

        let db = 
                let db = dbSchema.GetDataContext(connectionString)
                // Enable the logging of database activity to the console.
                db.DataContext.Log <- System.Console.Out
                db

        let fromEventDto (dto: dbSchema.ServiceTypes.ItemEvent) =
            let eventData = System.Text.Encoding.UTF8.GetBytes(dto.EventData);
            deserializeUnion dto.TypeName eventData
    
        let findEventDtos (AggregateId id) =
            db.ItemEvent |> Seq.filter (fun dto -> dto.StreamId = id) 

        let findEvents id =
            findEventDtos id |> Seq.choose fromEventDto

        let toEventDto(streamId, version, typeName, eventData) =
            dbSchema.ServiceTypes.ItemEvent(StreamId = streamId, EventNumber = version, TypeName = typeName, CreatedDate = DateTime.Now, EventData = eventData)

        let lastEventNumber id = 
            match findEvents id |> Seq.tryLast with
            | Some vers -> vers |> Events.versionFrom
            | _ -> AggregateVersion.Empty

        interface IItemsEventStore with
            member __.GetAll() =
                try
                    db.ItemEvent |> Seq.choose fromEventDto |> ok
                with 
                | ex -> fail (UnknownError ex.Message)

            member __.GetById(id) =
                try
                    findEvents id |> ok
                with 
                | ex -> fail (UnknownError ex.Message)

            member __.Record(e) = 
                try
                    // Todo: wrap inside a transaction
                    let streamId = Events.idFrom e
                    let version = Events.versionFrom e

                    match version <> AggregateVersion.incrVersion (lastEventNumber streamId) with
                    | false ->
                        serialize toEventDto (e, streamId, version)
                        |> db.ItemEvent.InsertOnSubmit
                        |> ignore
                        db.DataContext.SubmitChanges() |> ok
                    | true -> fail ConcurrencyFailure
                with
                | ex -> fail (UnknownError ex.Message)
