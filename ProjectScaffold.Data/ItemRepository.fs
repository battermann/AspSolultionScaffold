namespace ProjectScaffold.Data

module AccessLayer =
    open System
    open System.Data
    open System.Data.Linq
    open Microsoft.FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open ProjectScaffold.DomainModels
    open Chessie.ErrorHandling

    [<Literal>]
    let connectionString = "Data Source=(localdb)\V11.0;Initial Catalog=ProjectScaffold.Database;Integrated Security=SSPI;"
    type dbSchema = SqlDataConnection<connectionString>

    type ItemRepository(connectionString:string) =

        let db = 
            let db = dbSchema.GetDataContext(connectionString)
            // Enable the logging of database activity to the console.
            db.DataContext.Log <- System.Console.Out
            db

        let descriptionFrom =
            function
            | Some v -> v
            | _      -> null

        let fromItemDto (dto: dbSchema.ServiceTypes.Item): Item =
            { id = AggregateId dto.Id; version = AggregateVersion dto.Version; name = dto.Name; description = if dto.Description <> null then Some dto.Description else None }

        let toItemDto (AggregateId id, AggregateVersion version, description, name) =
            dbSchema.ServiceTypes.Item(Id = id, Version = version, Description = descriptionFrom description, Name = name)

        let tryFindItemDto (AggregateId itemId) =
            db.Item
            |> Seq.tryFind (fun dto -> dto.Id = itemId) 

        let tryFindItem =
            tryFindItemDto >> Option.map fromItemDto

        let versionFrom =
            function
            | AggregateVersion v -> v

        interface IItemReadAccess with
            
            member __.GetAll() =
                try
                    db.Item |> Seq.map fromItemDto |> ok
                with
                | ex -> Bad [SqlException ex.Message]

            member __.GetById(itemId) =
                try
                    match tryFindItem itemId with
                    | Some v -> ok v
                    | _      -> fail AggregateNotFound
                with
                | ex -> fail (SqlException ex.Message)

        interface IItemWriteAccess with

            member __.Update(e) =
                try 
                    // Todo: wrap inside a transaction
                    match e with
                    | ItemCreated msg -> 
                        match tryFindItem msg.id with
                        | None -> 
                            toItemDto (msg.id, msg.expectedVersion, msg.description, msg.name)
                            |> db.Item.InsertOnSubmit
                            |> ignore
                            db.DataContext.SubmitChanges()
                            ok()
                        | _ -> fail (DbUpdateError "item already exists")
                    | ItemUpdated msg -> 
                        match tryFindItemDto msg.id with
                        | Some dto -> 
                            match versionFrom msg.expectedVersion with
                            | v when v = dto.Version + 1 ->
                                dto.Name        <- msg.name
                                dto.Description <- descriptionFrom msg.description
                                dto.Version     <- v
                                db.DataContext.SubmitChanges()
                                ok()
                            | _ -> fail ConcurrencyFailure
                        | _ -> fail AggregateNotFound
                with
                | ex -> 
                    fail (DbUpdateError ex.Message)