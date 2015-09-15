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
    let db = dbSchema.GetDataContext()

    // Enable the logging of database activity to the console.
    db.DataContext.Log <- System.Console.Out

    let fromItemDto (dto: dbSchema.ServiceTypes.Item): Item =
        { id = ItemId dto.Id; name = dto.Name; description = if dto.Description <> null then Some dto.Description else None }

    type ItemRepository() =

        interface IItemReadAccess with
            
            member this.GetAll() =
                try
                    db.Item |> Seq.map fromItemDto |> ok
                with
                | ex -> Bad [SqlException ex.Message]

            member this.GetById(itemId) =
                try
                    let (ItemId id) = itemId
                    let item = db.Item |> Seq.tryFind (fun dto -> dto.Id = id) |> Option.map fromItemDto
                    match item with
                    | Some v -> ok v
                    | _ -> fail ItemNotFound
                with
                | ex -> fail (SqlException ex.Message)

        interface IItemWriteAccess with

            member this.Update(e) =
                try 
                    match e with
                    | ItemCreated msg -> 
                        let (ItemId id) = msg.id
                        let description: string = 
                            match msg.description with
                            | Some v -> v
                            | _ -> null
                        let fromDb = db.Item |> Seq.tryFind (fun dto -> dto.Id = id) |> Option.map fromItemDto
                        match fromDb with
                        | None -> 
                            let itemDto = new dbSchema.ServiceTypes.Item(Id = id, Description = description, Name = msg.name)
                            db.Item.InsertOnSubmit(itemDto) |> ignore
                            db.DataContext.SubmitChanges()
                            ok()
                        | _ -> fail (DbUpdateError "item already exists")
                    | ItemUpdated msg -> 
                        let (ItemId id) = msg.id
                        let description: string = 
                            match msg.description with
                            | Some v -> v
                            | _ -> null
                        let fromDb = db.Item |> Seq.tryFind (fun dto -> dto.Id = id)
                        match fromDb with
                        | Some dto -> 
                            dto.Name <- msg.name
                            dto.Description <-description
                            db.DataContext.SubmitChanges()
                            ok()
                        | _ -> fail (ItemNotFound)
                with
                | ex -> fail (DbUpdateError ex.Message)