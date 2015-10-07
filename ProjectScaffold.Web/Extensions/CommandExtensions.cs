using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Web.Extensions
{
    public static class CommandExtensions
    {
        public static Command ToCommand(this CreateIssue cmd)
        {
            return Command.NewIssueCommand(IssueCommand.NewCreateIssue(cmd));
        }

        public static Command ToCommand(this AddComment cmd)
        {
            return Command.NewIssueCommand(IssueCommand.NewAddComment(cmd));
        }
    }
}