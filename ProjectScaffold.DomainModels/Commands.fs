namespace ProjectScaffold.DomainModels

open System

type Commands =
    | IssueCommand of IssueCommand
and IssueCommand =
    | CreateIssue of CreateIssue
    | AddComment of AddComment
and CreateIssue = { Timestamp:DateTime; Id:IssueId; User:User }
and AddComment = { Timestamp:DateTime; Id:IssueId; User:User; Comment:string }