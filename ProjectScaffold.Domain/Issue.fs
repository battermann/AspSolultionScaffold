namespace ProjectScaffold.Domain

open System
open Chessie.ErrorHandling
open Helpers
open ProjectScaffold.DomainModels

type Issue =
    | Initial
    | InProgress of IssueState

and IssueState = { 
    Id:IssueId
    CreatedBy:User
    CreatedAt:DateTime
    Comments: string list }

module Issues = 

    let evolveOne state event = 
        match state, event with
        | Initial, IssueCreated data -> InProgress { Id = data.Id; CreatedBy = data.User; CreatedAt = data.Timestamp; Comments = []} |> ok
        | InProgress s, CommentAdded data -> InProgress { s with Comments = s.Comments @ [data.Comment]} |> ok
        | _ -> fail (InvalidStateTransition "Issue")

    let evolveIssue = evolve evolveOne

    let getIssueState deps id =
        snd
        <!> deps.readEvents IssueId.Prefix id
        >>= evolveIssue Initial

    let handleIssue deps cmd = 
        let createIssue (IssueId id) ts user svp = 
            match svp.State with
            | Initial -> (IssueId.Prefix, id, svp.Version, [IssueCreated { Timestamp = ts; Id = IssueId id; User = user }]) |> ok
            | _ -> fail (InvalidStateTransition "Issue")
        
        let addComment (IssueId id) ts user comment svp =
            match svp.State with
            | InProgress s -> (IssueId.Prefix, id, svp.Version, [CommentAdded { Timestamp = ts; Id = IssueId id; User = user; Comment = comment }]) |> ok
            | _ -> fail AggregateNotFound

        match cmd with
        | CreateIssue data -> getIssueState deps data.Id.Value >>= createIssue data.Id data.Timestamp data.User
        | AddComment data -> getIssueState deps data.Id.Value >>= addComment data.Id data.Timestamp data.User data.Comment