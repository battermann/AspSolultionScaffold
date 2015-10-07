using System;
using System.Net;
using System.Web.Mvc;
using Chessie.ErrorHandling;
using Chessie.ErrorHandling.CSharp;
using MvcFlashMessages;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Web.Extensions
{
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
                errs => (ActionResult) new HttpStatusCodeResult(code, msg));
        }

        public static T Success<T>(this Controller ctrl, string msg, T view)
        {
            ctrl.Flash("success", msg);
            return view;
        }
    }

    public static class GuidExtensions
    {
        public static Result<Guid, DomainMessage> TryParseGuid(this string s)
        {
            Guid guid;
            if (Guid.TryParse(s, out guid))
            {
                return Result<Guid, DomainMessage>.Succeed(guid);
            }

            return Result<Guid, DomainMessage>.FailWith(DomainMessage.NewInvalidOperation("Cannot parse string to guid"));
        }
    }
}