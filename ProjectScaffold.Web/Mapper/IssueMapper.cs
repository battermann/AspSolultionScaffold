using ProjectScaffold.Domain;
using ProjectScaffold.Web.Models;

namespace ProjectScaffold.Web.Mapper
{
    public static class IssueMapper
    {
        public static IssueVm Map(this Issue issue)
        {
            return issue.Match(
                onInitial: () => new IssueVm(), 
                onInProgress: state => new IssueVm
                    {
                        User = state.CreatedBy.Name.Item,
                        CreatedAt = state.CreatedAt,
                        Id = state.Id.Value.ToString(),
                        Comments = state.Comments
                    });
        }
    }
}