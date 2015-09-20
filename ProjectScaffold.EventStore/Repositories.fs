namespace ProjectScaffold.Data

module Repository =
    
    open System
    open System.Data
    open System.Data.Linq
    open Microsoft.FSharp.Linq
    open ProjectScaffold.DomainModels
    open Chessie.ErrorHandling
    open EventStore

    type ItemRepository(eventStore:IItemsEventStore) =
        
        let eventStore = eventStore

        let replay events = 
            events 
            |> Seq.sortBy Events.versionFrom
            |> Seq.fold Item.applyEvent (ok Item.initial)

        let isSuccess =
            function
            | Ok (v,_) -> true
            | Bad errs -> false

        interface IItemReadAccess with
            
            member __.GetAll() =
                eventStore.GetAll()
                >>= fun events -> 
                    events 
                    |> Seq.groupBy Events.idFrom 
                    |> Seq.map (snd >> replay) 
                    |> Seq.filter isSuccess 
                    |> Trial.collect 
                    |> Trial.lift Seq.ofList

            member __.GetById(id) =
                eventStore.GetById(id) 
                >>= replay
                >>= fun item ->
                    match Item.isEmpty item with
                    | true -> fail AggregateNotFound
                    | _ -> ok item

        interface IItemWriteAccess with 

            member __.Update(e) =
                eventStore.Record(e)

