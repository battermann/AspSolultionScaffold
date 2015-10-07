using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.FSharp.Collections;
using MvcFlashMessages;
using ProjectScaffold.Domain;
using ProjectScaffold.DomainModels;

namespace ProjectScaffold.Web.Extensions
{
    public static class DomainMessageExtensions
    {
        public static string ToMessageString(this DomainMessage msg)
        {
            return msg.Match(
                onInvalidStateTransition: s => String.Format("Invalid State Transition. {0}", s),
                onDbUpdateError: s => String.Format("Beim Schreiben in die Datenbank ist ein Fehler aufgetreten. {0}", s),
                onDbQueryError: s => String.Format("Beim Lesen aus der Datenbank ist ein Fehler aufgetreten. {0}", s),
                onConcurrencyFailure: () => "Anscheinend wurden Daten von einem anderen Benutzer geändert. Die Aktion konnte nicht ausgefürht werden.",
                onAggregateNotFound: () => "Das gesuchte Objekt konnte nicht gefunden werden.",
                onCouldNotLoadAggregate: s => string.Format("Das Objekt konnte nicht geladen werden. {0}", s),
                onAggregateAlreadyExists: () => "Das Objekt ist bereits vorhanden.",
                onInvalidOperation: s => string.Format("Die Operation kann nicht ausgeführt werden. {0}", s),
                onUnknownError: s => string.Format("Ein unbekannter Fehler ist aufgetreten. {0}", s));
        }

        public static IEnumerable<string> Convert(this FSharpList<DomainMessage> errs)
        {
            return errs.Select(ToMessageString);
        }

        public static ActionResult Failure(this Controller ctrl, FSharpList<DomainMessage> errs, ActionResult view)
        {
            foreach (var msg in errs.Convert())
            {
                ctrl.Flash("warning", msg);
            }

            return view;
        }
    }
}