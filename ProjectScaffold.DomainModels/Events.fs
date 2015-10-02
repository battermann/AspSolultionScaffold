namespace ProjectScaffold.DomainModels

open System

type Event =
    | IssueCreated of IssueCreated
    | CommentAdded of CommentAdded
// Issue event data
and IssueCreated = { Timestamp:DateTime; Id:IssueId; User:User } 
and CommentAdded = { Timestamp:DateTime; Id:IssueId; User:User; Comment:string }