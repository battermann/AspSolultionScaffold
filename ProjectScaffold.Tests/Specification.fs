namespace ProjectScaffold.Tests

module Specification =
    open System
    open ProjectScaffold.DomainModels
    open Chessie.ErrorHandling
    open ProjectScaffold.Data
    open ProjectScaffold.Domain
    open InMemoryEventStore
    open Helpers
    open CommandHandling
    open FsUnit

    let defaultDependencies = { readEvents = (fun prefix id -> Result<(int*Event list),DomainMessage>.Ok ((0,[]), [])); guidGenerator = Guid.NewGuid }

    let createTestApplication dependencies events = 
        let es = create()
        let toStreamId (prefix:string) (id:Guid) = sprintf "%s%O" prefix id
        let readStream prefix id = readFromStream es (toStreamId prefix id)

        events |> List.map (fun (prefix, id, evts) -> appendToStream es (toStreamId prefix id) -1 evts) |> ignore

        let deps = match dependencies with
                    | None -> { defaultDependencies with readEvents = readStream}
                    | Some d -> { d with readEvents = readStream }

        handle deps

    let Given (events, dependencies) = events, dependencies
    let When command (events, dependencies) = events, dependencies, command

    let Expect expectedEvents (events, dependencies, command) = 
        printfn "Given: %A" events
        printfn "When: %A" command
        printfn "Expects: %A" expectedEvents
        command 
        |> (createTestApplication dependencies events) 
        |> (fun (Ok ((prefix, id, version, events), msgs)) -> events)
        |> should equal expectedEvents

    let ExpectFail failure (events, dependencies, command) =
        printfn "Given: %A" events
        printfn "When: %A" command
        printfn "Should fail with: %A" failure

        command 
        |> (createTestApplication dependencies events) 
        |> (fun r -> r = fail failure)
        |> should equal true