using DigitalNagrik.Areas.GridView.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.GridView.Controllers
{
    public class AppealHistoryReportController : Controller
    {
        // GET: GridView/AppealHistoryReport

        [CheckSessions]
        public ActionResult Index()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            return View();
        }
        [CheckSessions]
        public ActionResult SearchAppeal()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            return View();
        }
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAppealDetails(SearchAppealDetails PostData)
        {
            try
            {            
                PostData = SearchAppealDetails.GetAppealDetails(PostData.SearchParameter, PostData.SearchText);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                TempData["Status"] = "F";
            }
            return PartialView("PartialViews/_AppealDetails", PostData);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppealHistoryReportPV(AppealHistoryReport PostData)
        {
            PostData = AppealHistoryReport.GetAppealHistoryReportData(PostData.FromDt, PostData.ToDt);
            return PartialView("_AppealHistoryReport", PostData);
        }

        public ActionResult ExportAppealHistoryReport(string qs, string ET)
        {
            string FromDt = string.Empty; string ToDt = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { FromDt = Params["FromDt"]; ToDt = Params["ToDt"]; } catch { }
            if (!string.IsNullOrWhiteSpace(ET))
            {
                try
                {
                    if (ET == "EXL")
                    {
                        DataTable DataTableExcel = AppealHistoryReport.GetAppealHistoryReportDataTable(FromDt, ToDt);
                        string[] ColumnNamesDB = { "AppealNo", "AppealDt", "AppellantNm", "IntermediaryNm", "MatterSummary", "GACTitle", "ObservationText", "IntermediaryResponse", "MemberComment_1", "MemberComment_2", "ChairpersonComent", "DraftDecision", "AppellantNotif" };
                        string[] ColumnNamesNew = { "Appeal No.", "Date of Appeal", "Name of Appellant", "Name of Intermediary", "Summary of Appeal", "GAC", "Assistant Manager/ Manager action/comment", "GO Response", "GAC Member-1 Comment", "GAC Member-2 Comment", "Chairperson’s Comment", "Final decision", "Notification Sent to Appellant" };
                        string[] ColumnNamesRemove = { "RegistrationYear", "GrievanceId", "IntermediaryFile", "AssignedToGAC", "ResponseText", "ResponseValue", "ResponseDetails", "ResponseURLs" };
                        DataTable dt = BALCommon.UpdateExcelDataTable(DataTableExcel, ColumnNamesDB, ColumnNamesNew, ColumnNamesRemove);
                        using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                        {
                            wb.Worksheets.Add(dt);
                            wb.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                            wb.Style.Font.Bold = true;
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename= AppealHistoryReport.xlsx");
                            using (System.IO.MemoryStream MyMemoryStream = new System.IO.MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
                        }
                    }
                    else if (ET == "PDF")
                    {
                        AppealHistoryReport PDFData = AppealHistoryReport.GetAppealHistoryReportData(FromDt, ToDt);
                        ViewData["AppealHistoryReportPdfString"] = BALCommon.RenderRazorViewToString("_AppealHistoryReport_PdfView", ControllerContext, ViewData, TempData, PDFData);
                        string CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"7\" --footer-left \"GAC (Appeal History Report)\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"";
                        ViewAsPdf PDF = new ViewAsPdf("_AppealHistoryReport_RenderReportPdf") { PageSize = Size.A4, PageOrientation = Orientation.Landscape, PageMargins = { Left = 6, Right = 6, Top = 6, Bottom = 6 }, CustomSwitches = CustomSwitches };
                        byte[] PDFBytes = PDF.BuildPdf(ControllerContext);
                        return File(PDFBytes, "application/pdf");
                    }
                    else if (ET == "WRD")
                    {
                        AppealHistoryReport WordData = AppealHistoryReport.GetAppealHistoryReportData(FromDt, ToDt);
                        string htmlText = BALCommon.RenderRazorViewToString("_AppealHistoryReport_Word", ControllerContext, ViewData, TempData, WordData);
                        Response.AppendHeader("Content-Type", "application/msword");
                        Response.AppendHeader("Content-disposition", "attachment; filename=AppealHistoryReport.doc");
                        Response.Write(htmlText.ToString());
                    }
                }
                catch (Exception ex) { MyExceptionHandler.HandleException(ex); }
            }
            return new EmptyResult();
        }
    }
}