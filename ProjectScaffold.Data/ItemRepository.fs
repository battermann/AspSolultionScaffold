namespace ProjectScaffold.Data

module AccessLayer =
    open System
    open System.Data
    open System.Data.Linq
    open Microsoft.FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open ProjectScaffold.DomainModels
    open Chessie.ErrorHandling

    type dbSchema = SqlDataConnection<"Data Source=(localdb)\V11.0;Initial Catalog=ProjectScaffold.Database;Integrated Security=SSPI;">

    type ItemRepository() =

        let db = 
            let db = dbSchema.GetDataContext()
            // Enable the logging of database activity to the console.
            db.DataContext.Log <- System.Console.Out
            db

        let descriptionFrom =
            function
            | Some v -> v
            | _      -> null

        let fromItemDto (dto: dbSchema.ServiceTypes.Item): Item =
            { id = ItemId dto.Id; name = dto.Name; description = if dto.Description <> null then Some dto.Description else None }

        let toItemDto (ItemId id, description, name) =
            dbSchema.ServiceTypes.Item(Id = id, Description = descriptionFrom description, Name = name)

        let tryFindItemDto (ItemId itemId) =
            db.Item
            |> Seq.tryFind (fun dto -> dto.Id = itemId) 

        let tryFindItem =
            tryFindItemDto >> Option.map fromItemDto

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
                    | _      -> fail ItemNotFound
                with
                | ex -> fail (SqlException ex.Message)

        interface IItemWriteAccess with

            member __.Update(e) =
                try 
                    match e with
                    | ItemCreated msg -> 
                        match tryFindItem msg.id with
                        | None -> 
                            toItemDto (msg.id, msg.description, msg.name)
                            |> db.Item.InsertOnSubmit
                            |> ignore
                            db.DataContext.SubmitChanges()
                            ok()
                        | _ -> fail (DbUpdateError "item already exists")
                    | ItemUpdated msg -> 
                        match tryFindItemDto msg.id with
                        | Some dto -> 
                            dto.Name        <- msg.name
                            dto.Description <- descriptionFrom msg.description
                            db.DataContext.SubmitChanges()
                            ok()
                        | _ -> fail (ItemNotFound)
                with
                | ex -> 
                    fail (DbUpdateError ex.Message)