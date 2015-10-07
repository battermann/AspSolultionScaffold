namespace ProjectScaffold.Tests

open System
open ProjectScaffold.DomainModels
open ProjectScaffold.Domain
open Chessie.ErrorHandling
open FsUnit
open NUnit.Framework
open Specification

module ``When creating an issue`` =
    
    [<Test>]
    let ``an issue should be created`` () =
        let id = Guid.NewGuid()
        let ts = new DateTime(2015,1,1)
        Given([], None)
        |> When (Command.IssueCommand(CreateIssue({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"} })))
        |> Expect [IssueCreated({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"}} )]

    [<Test>]
    let ``it fails if issue already exists`` () =
        let id = Guid.NewGuid()
        let ts = new DateTime(2015,1,1)
        Given([IssueId.Prefix, id, [IssueCreated({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"}} )]], None)
        |> When (Command.IssueCommand(CreateIssue({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"} })))
        |> ExpectFail (InvalidStateTransition "Issue")

module ``When adding a comment`` =

    [<Test>]
    let ``it fails if issue does not exists`` () =
        let id = Guid.NewGuid()
        let ts = new DateTime(2015,1,1)
        Given([], None)
        |> When (Command.IssueCommand(AddComment({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"}; Comment = "foo" })))
        |> ExpectFail AggregateNotFound 
        
    [<Test>]
    let ``a comment should be added`` () =
        let id = Guid.NewGuid()
        let ts = new DateTime(2015,1,1)
        Given([IssueId.Prefix, id, [IssueCreated({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"}} )]], None)
        |> When (Command.IssueCommand(AddComment({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"}; Comment = "foo" })))
        |> Expect [CommentAdded({Timestamp = ts; Id = IssueId id; User = { Name = UserName "admin"}; Comment = "foo" } )] 

