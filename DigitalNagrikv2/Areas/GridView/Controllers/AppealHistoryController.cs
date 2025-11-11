using DigitalNagrik.Areas.GridView.Data;
using DigitalNagrik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalNagrik.Filters;

namespace DigitalNagrik.Areas.GridView.Controllers
{
    public class AppealHistoryController : Controller
    {
        // GET: GridView/AppealHistory
        [CheckSessions]
        public ActionResult AppealHistoryPV(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            if (!string.IsNullOrWhiteSpace(RegistrationYear) && !string.IsNullOrWhiteSpace(GrievanceId))
            {
                AppealHistory Appeal = AppealHistory.GetAppealHistory(RegistrationYear, GrievanceId); Appeal.RegistrationYear = RegistrationYear; Appeal.GrievanceId = GrievanceId;
                return PartialView("_AppealHistory", Appeal);
            }
            return new EmptyResult();
        }
    }
}