namespace ProjectScaffold.Domain

open System
open Chessie.ErrorHandling
open ProjectScaffold.DomainModels

module Helpers =
    
    type Dependencies = {readEvents: string -> Guid -> Result<(int*Event list), DomainMessage>; guidGenerator: unit -> Guid}

    type StateVersionPair<'a> = { State:'a; Version:int }

    let evolve evolveOne initState  =
        List.fold (fun result e -> 
              result >>= fun svp ->
                  evolveOne svp.State e >>= fun s -> ok ({ Version = svp.Version + 1; State = s })) 
            (ok ({ Version = -1; State = initState })) 

    let getTypeName o = o.GetType().Name
    let stateTransitionFail event state = fail (InvalidStateTransition (sprintf "Invalid event %s for state %s" (event |> getTypeName) (state |> getTypeName)))