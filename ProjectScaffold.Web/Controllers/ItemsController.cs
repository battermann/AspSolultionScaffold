using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Chessie.ErrorHandling;
using Chessie.ErrorHandling.CSharp;
using InternalMessageBus;
using ProjectScaffold.DomainModels;
using WebProjectScaffold.Web.Models;
using FSharpx;

namespace WebProjectScaffold.Web.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemReadAccess _repository;
        private readonly ICommandSender _commandSender;

        public ItemsController(IItemReadAccess repository, ICommandSenderFactory factory)
        {
            _repository = repository;
            _commandSender = factory.CommandSender;
        }

        // GET: /Items/ 
        public ActionResult Index()
        {
            var result = _repository.GetAll();

            return result.ViewOrBadRequest(items => View(items.Select(x => new ItemVm() { Id = x.id.Item, Name = x.name, Description = x.description.GetOrElse((string)null) })));
        }

        // GET: /Items/Edit/id
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var result = _repository.GetById(AggregateId.NewAggregateId(id));

            return result.ViewOrNotFound(item => View(new ItemVm() { Id = item.id.Item, Name = item.name, Description = item.description.GetOrElse((string)null) }));
        }

        // POST: /Items/Edit/id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemVm vm)
        {
            if (ModelState.IsValid)
            {
                var cmd = new UpdateItem(
                    timestamp: DateTime.Now,
                    id: AggregateId.NewAggregateId(vm.Id),
                    name: vm.Name,
                    description: vm.Description.Some());

                _commandSender.Send(cmd);
            }

            return RedirectToAction("Index");
        }

        // GET: /Items/Create/id
        public ActionResult Create()
        {
            return View(new ItemVm { Id = Guid.NewGuid().ToString(), Name = "Enter a name", Description = "Enter a description" });
        }

        // POST: /Items/Create/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemVm vm)
        {
            if (ModelState.IsValid)
            {
                var cmd = new CreateItem(
                    timestamp: DateTime.Now,
                    id: AggregateId.NewAggregateId(vm.Id),
                    name: vm.Name,
                    description: vm.Description.Some());

                _commandSender.Send(cmd);
            }

            return RedirectToAction("Index");
        }
    }

    public static class FunctionalHttpExtensions
    {
        public static ActionResult ViewOrBadRequest<TSuccess, TMessage>(this Result<TSuccess, TMessage> result, Func<TSuccess, ActionResult> f, string msg = null)
        {
            return ActionResult(result, f, HttpStatusCode.BadRequest, msg);
        }

        public static ActionResult ViewOrNotFound<TSuccess, TMessage>(this Result<TSuccess, TMessage> result, Func<TSuccess, ActionResult> f, string msg = null)
        {
            return ActionResult(result, f, HttpStatusCode.NotFound, msg);
        }

        private static ActionResult ActionResult<TSuccess, TMessage>(Result<TSuccess, TMessage> result, Func<TSuccess, ActionResult> f, HttpStatusCode code, string msg = null)
        {
            return result.Either(
                (v, _) => f(v),
                errs => (ActionResult)new HttpStatusCodeResult(code, msg));
        }
    }
}