using ClosedXML.Excel;
using DigitalNagrik.Areas.Dashboard.Models;
using DigitalNagrik.Areas.GridView.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Areas.Dashboard.Controllers
{
    public class AdminDashboardController : Controller
    {
        // GET: Dashboard/AdminDashboard

        [CheckSessions]
        public ActionResult Index()
        {
            if (UserSession.RoleID.Equals("1") || UserSession.RoleID.Equals("2") || UserSession.RoleID.Equals("5")|| UserSession.RoleID.Equals("7") || UserSession.RoleID.Equals("10"))
            //
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
                AdminDashboard Data = new AdminDashboard { GACList = AdminDashboard.GetMappedGACList(UserSession.UserID) };
                Data.GACId = Data.GACList.FirstOrDefault().Value;
                return View(Data);
            }
            else
            {
                //RedirectToAction("Error", "Home", new { Area = "" });
                return RedirectToAction("Error", "Home", new { Area = "" });
            }

        }

        [CheckSessions]
        public ActionResult GetDashboardStats_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            string GACId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { GACId = Params["GACId"]; } catch { }
            AdminDashboard Data = new AdminDashboard
            {
                Stats = Dashboard_Stats.GetData(GACId),
                SixMonths = Dashboard_6Months.GetData(GACId),
                GACStats = Dashboard_GACStats.GetData(GACId),
                ReliefSought = Dashboard_ReliefSought.GetData(GACId),
                AppealGround = Dashboard_AppealGround.GetData(GACId),
                GACId = GACId
            };
            return View("_AdminDashboard", Data);
        }

        [CheckSessions]
        public ActionResult GetAppealList_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            string GACId = string.Empty; string Flag = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { Flag = Params["Flag"]; GACId = Params["GACId"]; } catch { }
            DateTime TodayDate = DateTime.Today; string ToDt = TodayDate.ToString("dd/MM/yyyy").Replace("-", "/"); string FromDt = TodayDate.AddDays(-30).ToString("dd/MM/yyyy").Replace("-", "/");
            AdminDashboard_AppealList Data = AdminDashboard_AppealList.GetAppealList(GACId, Flag, FromDt, ToDt); Data.ToDt = ToDt; Data.FromDt = FromDt;
            Data.IntermediaryList = GACIntermediaryVerification.GetIntermediaryList();
            return View("_AppealList", Data);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAppealListFiltered_PV(AdminDashboard_AppealList PostData)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            AdminDashboard_AppealList Data = AdminDashboard_AppealList.GetAppealList(PostData.GACId, PostData.Flag, PostData.FromDt, PostData.ToDt, PostData.IntermediaryId, PostData.IsCompliance, PostData.IsIntermediaryResponse, PostData.AssignmentStatus);
            return View("_FilteredAppealList", Data);
        }

        [CheckSessions]
        public ActionResult GetAppealAssignmentForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            Dashboard_AssignAppeal Action = Dashboard_AssignAppeal.GetAppealInfo(RegistrationYear, GrievanceId);
            Action.GACList = Dashboard_AssignAppeal.GetList("GAC_LIST"); Action.AppealGroundList = Dashboard_AssignAppeal.SelectListItemAppealGround.GetGroundList();
            return PartialView("Forms/_AppealAssignment", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MeityAssignAppeal(Dashboard_AssignAppeal PostData)
        {
            TempData["Status"] = "F";
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@GroundId", PostData.AppealGround),
                    new KeyValuePair<string, string>("@GACId", PostData.GACId),
                    new KeyValuePair<string, string>("@Remarks", Trim(PostData.Remarks)),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ASSIGN_APPEAL")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_AppealAssignment_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                string GACNm = Convert.ToString(ds.Tables[0].Rows[0]["GACNm"]);
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture, "Appealsuccessfullyassigned") + " (<span class='fw-600'>" + GACNm + "</span>) (Appeal No. : <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard" });
        }

        //Dashboard Statistics

        [CheckSessions]
        public ActionResult Statistics_PV()
        {
            Dashboard_Stats Data = Dashboard_Stats.GetData(UserSession.UserID);
            return View("PartialViews/_Stats", Data);
        }

        [CheckSessions]
        public ActionResult StatisticsAverageDays_PV(string GACId, string Flag)
        {
            if (Flag.Length > 2)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            Dashboard_Stats_AverageDays Data = Dashboard_Stats_AverageDays.GetData(GACId, Flag.ToString());
            if (Flag != GACGridViewInbox.MenuStatus.Inbox)
            {
                Data.HtmlString = BALCommon.RenderRazorViewToString("PartialViews/_Stats_AverageDays", ControllerContext, ViewData, TempData, Data);
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        [CheckSessions]
        public ActionResult MeityAvgDays_PV()
        {
            Dashboard_AvgDays Data = Dashboard_AvgDays.GetData(UserSession.UserID);
            return View("PartialViews/_AvgDays", Data);
        }

        [CheckSessions]
        public ActionResult Meity6Months_PV()
        {
            Dashboard_6Months Data = Dashboard_6Months.GetData(UserSession.UserID);
            return View("PartialViews/_6Months", Data);
        }

        [CheckSessions]
        public ActionResult MeityGACStats_PV()
        {
            Dashboard_GACStats Data = Dashboard_GACStats.GetData(UserSession.UserID);
            return View("PartialViews/_GACStats", Data);
        }

        [CheckSessions]
        public ActionResult ReliefSought_PV()
        {
            Dashboard_ReliefSought Data = Dashboard_ReliefSought.GetData(UserSession.UserID);
            return View("PartialViews/_ReliefWise", Data);
        }

        [CheckSessions]
        public ActionResult GroundWise_PV()
        {
            Dashboard_AppealGround Data = Dashboard_AppealGround.GetData(UserSession.UserID);
            return View("PartialViews/_GroundWise", Data);
        }
        //New Reports
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadExcelAdminList(AdminDashboard_AppealList PostData)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            byte[] fileContents = null; // Initialize as null to handle the case if file generation fails

            // Fetch data based on the filter parameters
            DataSet ds = AdminDashboard_AppealList.GetAppealListData(PostData.GACId, PostData.Flag, PostData.FromDt, PostData.ToDt, PostData.IntermediaryId, PostData.IsCompliance, PostData.IsIntermediaryResponse, PostData.AssignmentStatus);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string[] DBColumnNames = new[] { "ReceiptDate", "GrievanceNo", "GrievanceDesc", "IntermediaryTitle", "IntermediaryURL", "IsIntermediaryReply", "IntermediaryReplyDateTime", "IntermediaryReplyTimeTaken", "IntermediaryReplyTimeLeft", "GrievnaceStatus" };
                string[] NewColumnNames = new[] { "Receipt Date", "Grievance No", "Grievance Desc", "Intermediary Title", "Intermediary URL", "Is Intermediary Reply", "IntermediaryReplyDateTime", "IntermediaryReplyTimeTaken", "IntermediaryReplyTimeLeft", "GrievnaceStatus" };
                string[] ColumnsToRemoveNew = new[] { "LastActionDateTime", "RegistrationYear", "GrievanceId" };

                System.Data.DataTable dtDataTable1 = BALCommon.UpdateExcelDataTable(ds.Tables[0], DBColumnNames, NewColumnNames, ColumnsToRemoveNew);

                using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("AppealList");
                    ws.Cell(1, 1).Value = "Appeal List";
                    ws.Range(1, 1, 1, 8).Merge().AddToNamed("Titles");

                    var tableWithData = ws.Cell(2, 1).InsertTable(dtDataTable1.AsEnumerable());

                    var titlesStyle = wb.Style;
                    titlesStyle.Font.Bold = true;
                    titlesStyle.Font.FontSize = 14;
                    titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titlesStyle.Fill.BackgroundColor = XLColor.Navy;
                    titlesStyle.Font.FontColor = XLColor.White;
                    wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

                    ws.Columns().AdjustToContents();
                    wb.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    using (System.IO.MemoryStream MyMemoryStream = new System.IO.MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);  // Save the Excel file to memory stream
                        fileContents = MyMemoryStream.ToArray(); // Get the byte array containing the Excel file
                    }
                }
            }

            // Check if fileContents is populated with data, else return an empty response or an error message
            if (fileContents == null || fileContents.Length == 0)
            {
                return new HttpStatusCodeResult(400, "No data available to generate Excel file.");
            }

            // Return the Excel file to the client with the correct MIME type and filename
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredAppealList.xlsx");
        }
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadPDFAdminList(AdminDashboard_AppealList PostData)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            AdminDashboard_AppealList Data = AdminDashboard_AppealList.GetAppealList(PostData.GACId, PostData.Flag, PostData.FromDt, PostData.ToDt, PostData.IntermediaryId, PostData.IsCompliance, PostData.IsIntermediaryResponse, PostData.AssignmentStatus);
            ViewAsPdf PDF = new ViewAsPdf("Forms/_AppealListPDF", Data)
            {
                FileName = "FilteredAppealList.pdf",
                PageMargins = { Left = 8, Right = 8 },
                CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
            byte[] PDFBytes = PDF.BuildPdf(ControllerContext);
            return File(PDFBytes, "application/pdf");
        }
        //New Reports
    }
}