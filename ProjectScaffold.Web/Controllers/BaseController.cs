using System.Web.Mvc;

namespace ProjectScaffold.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly DomainApi Api;

        protected BaseController()
        {
            Api = DomainApiBootstrapper.ResolveDomainApi();
        }
    }
}