using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Chessie.ErrorHandling;
using Chessie.ErrorHandling.CSharp;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using ProjectScaffold.Data;
using ProjectScaffold.Domain;
using ProjectScaffold.DomainModels;
using ProjectScaffold.Web.Extensions;

namespace ProjectScaffold.Web
{
    public static class DomainApiBootstrapper
    {
        public static DomainApi ResolveDomainApi()
        {
            var toStreamId = new Func<string, Guid, string>((prefix, id) => String.Format("{0}{1}", prefix, id.ToString()));
            var settings = ConfigurationManager.ConnectionStrings;
            var connectionString = settings["EventStore"].ConnectionString;

            var appendToStream = new Func<string, int, FSharpList<Event>, Result<Unit, DomainMessage>>(
                (streamId, expectedVersion, events) => EventStore.appendToStream(connectionString, streamId, expectedVersion, events));

            var readStream = new Func<string, Guid, Result<Tuple<int, FSharpList<Event>>, DomainMessage>>(
                (prefix, id) => EventStore.readFromStream<string, Event>(connectionString, toStreamId(prefix, id)));

            var deps = new Helpers.Dependencies(readStream.FromFunc(), new Func<Guid>(Guid.NewGuid).FromFunc());

            var save = new Func<string, Guid, int, FSharpList<Event>, Result<List<Event>, DomainMessage>>(
                (prefix, guid, version, events) => appendToStream(toStreamId(prefix, guid), version, events)
                    .Select(x => events.ToList()));

            var handle = new Func<Command, Result<List<Event>, DomainMessage>>(
                cmd => CommandHandling.handle(deps, cmd)
                    .SelectMany(tuple => save(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)));

            var issues = new Func<Result<List<Issue>, DomainMessage>>(
                () => EventStore.getStreamIds(IssueId.Prefix, connectionString)
                    .SelectMany(ids => ids
                        .Select(id => id.Substring(IssueId.Prefix.Length))
                        .Select(id => id.TryParseGuid().SelectMany(guid => Issues.getIssueState(deps, guid).Select(svp => svp.State)))
                        .Collect()
                        .Select(iss => iss.ToList())));

            var issue = new Func<string, Result<Issue, DomainMessage>>(
                id => id.TryParseGuid()
                    .SelectMany(guid => Issues.getIssueState(deps, guid)
                    .SelectMany(svp => svp.State
                        .Match(
                            onInitial: () => Result<Issue, DomainMessage>.FailWith(DomainMessage.AggregateNotFound),
                            onInProgress: state => Result<Issue, DomainMessage>.Succeed(svp.State)))));

            return new DomainApi(handle, issues, issue);
        }
    }
}