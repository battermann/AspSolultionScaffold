using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chessie.ErrorHandling.CSharp;
using MvcFlashMessages;
using ProjectScaffold.Web.Extensions;
using ProjectScaffold.Web.Mapper;
using ProjectScaffold.Web.Models;
using ProjectScaffold.DomainModels;
using ProjectScaffold.Domain;

namespace ProjectScaffold.Web.Controllers
{
    public class IssuesController : BaseController
    {
        // GET: Issues
        public ActionResult Index()
        {
            var issues = Api.Issues();

            return issues.Either(
                ifSuccess: (iss, messages) => View(iss.Select(IssueMapper.Map)),
                ifFailure: errs => this.Failure(errs, View(new List<IssueVm>())));
        }

        // GET: /Issues/Create
        public ActionResult Create()
        {
            return View(new CreateIssueVm { Id = Guid.NewGuid().ToString() });
        }

        // GET: /Issues/Edit/id
        public ActionResult Details(string id)
        {
            if (id != null)
            {
                var issue = Api.Issue(id);
                
                return issue.Either(    
                    ifSuccess: (i, messages) => (ActionResult)View(i.Map()),
                    ifFailure: errs => this.Failure(errs, RedirectToAction("Index")));
            }

            this.Flash("warning", "Die Id ist ungültig.");

            return RedirectToAction("Index");
        }

        // GET: /Issues/Edit/id
        public ActionResult Edit(string id)
        {
            if (id != null)
            {
                return View(new IssueAddCommentVm { Id = id });
            }

            this.Flash("warning", "Die Id ist ungültig.");

            return RedirectToAction("Index");
        }

        // POST: /Issues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateIssueVm vm)
        {
            if (ModelState.IsValid)
            {
                var cmd = vm.Id
                    .TryParseGuid()
                    .Select(guid => new CreateIssue(
                        timestamp: DateTime.Now,
                        id: IssueId.NewIssueId(guid),
                        user: new User(UserName.NewUserName("default user"))));

                var result = cmd.SelectMany(x => Api.Handle(x.ToCommand()));

                return result.Either(
                    ifSuccess: (e, messages) => this.Success("Issue wurde erfolgreich erstellt.", RedirectToAction("Index")),
                    ifFailure: errs => this.Failure(errs, RedirectToAction("Index")));
            }

            return RedirectToAction("Index");
        }

        // POST: /Issues/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IssueAddCommentVm vm)
        {
            if (ModelState.IsValid)
            {
                var cmd = vm.Id
                    .TryParseGuid()
                    .Select(guid => new AddComment(
                        timestamp: DateTime.Now,
                        id: IssueId.NewIssueId(guid),
                        user: new User(UserName.NewUserName("default user")),
                        comment: vm.Comment));

                var result = cmd.SelectMany(x => Api.Handle(x.ToCommand()));

                return result.Either(
                    ifSuccess: (e, messages) => this.Success("Kommentar wurde erfolgreich hinzugefügt.", RedirectToAction("Index")),
                    ifFailure: errs => this.Failure(errs, RedirectToAction("Index")));
            }

            return RedirectToAction("Index");
        }
    }
}