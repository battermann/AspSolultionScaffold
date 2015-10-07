namespace ProjectScaffold.Domain

open ProjectScaffold.DomainModels
open Helpers
open Issues

module CommandHandling =
    let handle (deps:Dependencies) = function
    | Command.IssueCommand(cmd) -> handleIssue deps cmd