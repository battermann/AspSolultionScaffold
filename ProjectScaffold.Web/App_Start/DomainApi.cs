using System;
using System.Collections.Generic;
using Chessie.ErrorHandling;
using ProjectScaffold.Domain;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Web
{
    public class DomainApi
    {
        public DomainApi(Func<Command, Result<List<Event>, DomainMessage>> handle, Func<Result<List<Issue>, DomainMessage>> issues, Func<string, Result<Issue, DomainMessage>> issue)
        {
            Issue = issue;
            Issues = issues;
            Handle = handle;
        }

        public Func<Command, Result<List<Event>, DomainMessage>> Handle { get; private set; }

        public Func<Result<List<Issue>, DomainMessage>> Issues { get; private set; }
        public Func<string, Result<Issue, DomainMessage>> Issue { get; private set; }
    }
}