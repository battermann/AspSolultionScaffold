using System;
using System.Collections.Generic;

namespace ProjectScaffold.Web.Models
{
    public class IssueVm
    {
        public string Id { get; set; }
        public string User { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> Comments { get; set; }
    }
}