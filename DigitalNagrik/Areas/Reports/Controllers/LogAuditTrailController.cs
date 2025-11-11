using ClosedXML.Excel;
using DigitalNagrik.Areas.Reports.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Reports.Controllers
{
    public class LogAuditTrailController : Controller
    {
        // GET: Dashboard/AdminDashboard
        [CheckSessions]
        public ActionResult Index()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Reports, "1", UserSession.LangCulture);
            return View();
        }
        [CheckSessions]
        public ActionResult Header_PV()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Reports, "1", UserSession.LangCulture);
            LogAuditTrailParams Data = LogAuditTrailParams.GetData();
            return View("PartialViews/_Header", Data);
        }
        [CheckSessions]
        public ActionResult DataList(string UserID = "", string UserTypeID = "", string FromDate = "", string ToDate = "")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Reports, "1", UserSession.LangCulture);
            LogAuditTrailReportData Data = LogAuditTrailReportData.GetData(UserID, UserTypeID, FromDate, ToDate);
            ViewBag.UserID = UserID;ViewBag.UserTypeID = UserTypeID; ViewBag.FromDate = FromDate; ViewBag.ToDate = ToDate;
            return View("PartialViews/_DataList", Data);

        }
        public JsonResult GetUsers(string UserTypeID)
        {
            return Json(getUserList(UserTypeID), JsonRequestBehavior.AllowGet);
        }
        public static List<SelectListItem> getUserList(string UserTypeID)
        {
            List<SelectListItem> _UserLIst = new List<SelectListItem>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserTypeID", UserTypeID),
                    new KeyValuePair<string, string>("@Mode", "UserList"),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "LogAuditTrailReport", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        _UserLIst.Add(new SelectListItem { Text = Convert.ToString(dr["UserName"]), Value = Convert.ToString(dr["UserID"]) });
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return _UserLIst;
        }
        public ActionResult LogAuditTrailReportDownloadExcel(string qs)
        {
            Dictionary<string, string> Params = CommonRepository.GetQSDecryptedParameters(qs);
            string UserID = Params["UserID"];
            string UserTypeID = Params["UserTypeID"];
            string FromDate = Params["FromDate"];
            string ToDate = Params["ToDate"];
            DataSet ds = new DataSet();
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@Mode", "DataList"),
                    new KeyValuePair<string, string>("@UserID", UserID),
                    new KeyValuePair<string, string>("@UserTypeID", UserTypeID),
                    new KeyValuePair<string, string>("@FromDate", FromDate),
                    new KeyValuePair<string, string>("@ToDate", ToDate),
                 };
                ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "LogAuditTrailReport", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string[] DBColumnNames = new[] { "UserID", "UserName", "AuthenticationStatus", "LoginDateTime", "LogoutDateTime", "IPAddress", "UserType" };
                    string[] NewColumnNames = new[] { "User ID", "User Name", "Authentication Status", "Login Date Time", "Logout Date Time", "IP Address", "User Type" };
                    string[] ColumnsToRemoveNew = new[] { "LoginDate", "StartTime", "LogoutDate", "EndTime", "NewLoginDateTime" };

                    // Dim ColumnsToRemoveNew As String() = {}
                    System.Data.DataTable dtDataTable1 = BALCommon.UpdateExcelDataTable(ds.Tables[0], DBColumnNames, NewColumnNames, ColumnsToRemoveNew);

                    using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("LogAuditTrail");
                        ws.Cell(1, 1).Value = "Log Audit Trail";
                        ws.Range(1, 1, 1, 7).Merge().AddToNamed("Titles");
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
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=LogAuditTrail.xlsx");

                        using (System.IO.MemoryStream MyMemoryStream = new System.IO.MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            // ltMessage.Text = "<div class='col-lg-12 alert alert-success'><b>Excel file has been generated for " + Records + " records.</b></div>"
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return new EmptyResult();
        }

    }
}