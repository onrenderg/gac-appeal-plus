using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace DigitalNagrik.Controllers
{
    public class GACAppealController : Controller
    {
        // GET: GACAppeal
        [CheckSessions]
        public ActionResult ShowTracker(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            AppealTrackDetModl obj = new AppealTrackDetModl();
            obj.AppealTrackDetlst = new List<AppealTrackDet>();
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            var methodparameter = new List<KeyValuePair<string, string>>();
            methodparameter.Add(new KeyValuePair<string, string>("@grievanceId", GrievanceId));
            methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetAppeal_trackDet", methodparameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Dr in ds.Tables[0].Rows)
                    {
                        obj.AppealTrackDetlst.Add(new AppealTrackDet
                        {
                            RegYear = Dr["RegistrationYear"].ToString(),
                            GrieveID = Dr["GrievanceId"].ToString(),
                            GrievanceDesc = Dr["GrievanceDesc"].ToString(),
                            ActionID = Dr["ActionId"].ToString(),
                            Remarks = Dr["Remarks"].ToString(),
                            ActionBy = Dr["ActionBy"].ToString(),
                            ActionDate = Dr["actiondt"].ToString(),
                            ActionIp = Dr["ActionByIP"].ToString(),
                            ActionTitle = Dr["ActionTitle"].ToString(),
                            ActionTime = Dr["actiontime"].ToString(),
                            Actionlvl = Dr["ActionLevel"].ToString(),
                            GacAbbr = Dr["gacabbr"].ToString(),
                            GacTitle = Dr["GACTitle"].ToString(),
                            AssignedTo = Dr["assignedto"].ToString(),
                            RejectReason = Dr["NoAdmitTypeDesc"].ToString(),
                            RejectSummary = Dr["RejectSummary"].ToString(),
                        });
                    }
                }
            }
            return PartialView("ShowTracker", obj);
        }

        public ActionResult ShowTrackerCitizen(string qs)
        {
            AppealTrackDetModl obj = new AppealTrackDetModl();
            obj.AppealTrackDetlst = new List<AppealTrackDet>();
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            var methodparameter = new List<KeyValuePair<string, string>>();
            methodparameter.Add(new KeyValuePair<string, string>("@grievanceId", GrievanceId));
            methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetAppeal_trackDet", methodparameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Dr in ds.Tables[0].Rows)
                    {
                        obj.AppealTrackDetlst.Add(new AppealTrackDet
                        {
                            RegYear = Dr["RegistrationYear"].ToString(),
                            GrieveID = Dr["GrievanceId"].ToString(),
                            GrievanceDesc = Dr["GrievanceDesc"].ToString(),
                            ActionID = Dr["ActionId"].ToString(),
                            Remarks = Dr["Remarks"].ToString(),
                            ActionBy = Dr["ActionBy"].ToString(),
                            ActionDate = Dr["actiondt"].ToString(),
                            ActionIp = Dr["ActionByIP"].ToString(),
                            ActionTitle = Dr["ActionTitle"].ToString(),
                            ActionTime = Dr["actiontime"].ToString(),
                            Actionlvl = Dr["ActionLevel"].ToString(),
                            GacAbbr = Dr["gacabbr"].ToString(),
                            GacTitle = Dr["GACTitle"].ToString(),
                            AssignedTo = Dr["assignedto"].ToString(),
                        });
                    }
                }
            }
            return PartialView("ShowTrackerCitizen", obj);
        }
    }
}