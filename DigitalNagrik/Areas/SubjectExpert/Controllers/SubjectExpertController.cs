using ClosedXML.Excel;
using DigitalNagrik.Areas.SubjectExpert.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DigitalNagrik.Areas.SubjectExpert.Controllers
{
    public class SubjectExpertController : Controller
    {
        // GET: SubjectExpert/SubjectExpert
        [CheckSessions]
        public ActionResult Index()
        {
            if (UserSession.RoleID.Equals("97"))
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.SubjectExpert, "1", UserSession.LangCulture);
                return View();
            }
            else
            {
                //RedirectToAction("Error", "Home", new { Area = "" });
                return RedirectToAction("Error", "Home", new { Area = "" });
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Statistics_PV()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.SubjectExpert, "1",UserSession.LangCulture);
            SubjectExpertDash_Stats Data = SubjectExpertDash_Stats.GetData(UserSession.Mobile);
            return View("PartialViews/_Stats", Data);
        }
        public ActionResult ListofAppeals(string ListType = "PENDING")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.SubjectExpert, "1",UserSession.LangCulture);
            SubjectExpert_AppealList Data = SubjectExpert_AppealList.GetData(UserSession.Mobile, ListType, (Dictionary<string, string>)ViewData["LabelDictionary"]);                      
                return View("PartialViews/_ListofAppeals", Data);
        }
        //------------------------------Add Expert----------------------------//
        [CheckSessions]
        public ActionResult AddNewExpert()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters,FormLabels.FormMasters.MasterExpertUsers, UserSession.LangCulture);
            Experts obj = new Experts();
            obj.List = new List<ExpertsList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    //methodparameter.Add(new KeyValuePair<string, string>("@GrievanceId", GrievanceId));
                    //methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetExpertList]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new ExpertsList
                            {
                                AreaofExpertise = Convert.ToString(dr["AreaofExpertise"]),
                                EmailID = Convert.ToString(dr["EmailID"]),
                                Mobile = Convert.ToString(dr["Mobile"]),
                                UserID = Convert.ToString(dr["UserID"]),
                                UserName = Convert.ToString(dr["UserName"]),
                                IsActive = Convert.ToString(dr["IsActive"])
                            });
                        }

                    }
                }
            }

            catch (Exception ex)
            {

            }
            return View("AddNewExpert", obj);
        }
        [CheckSessions]
        public ActionResult AddUpdateExpertShowPopUp(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters,FormLabels.FormMasters.MasterExpertUsers, UserSession.LangCulture);
            string UserId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { UserId = Params["UserId"]; } catch { }
            ExpertsList obj = new ExpertsList();
            obj = AddUpdateExpertShowPopUpData(UserId);
            return PartialView("PartialViews/_AddUpdateExpert", obj);
        }
        [CheckSessions]
        public ExpertsList AddUpdateExpertShowPopUpData(string UserID)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters,FormLabels.FormMasters.MasterExpertUsers, UserSession.LangCulture);
            ExpertsList obj = new ExpertsList();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetExpertList", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.AreaofExpertise = Convert.ToString(ds.Tables[0].Rows[0]["AreaofExpertise"]);
                        obj.EmailID = Convert.ToString(ds.Tables[0].Rows[0]["EmailID"]);
                        obj.IsActive = Convert.ToString(ds.Tables[0].Rows[0]["IsActive"]);
                        obj.Mobile = Convert.ToString(ds.Tables[0].Rows[0]["Mobile"]);
                        obj.UserID = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]);
                        obj.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [CheckSessions]
        public JsonResult ChangeStatusisActive(string UserID, string Command)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserID),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Expert_Commands", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = "S";
                                break;
                            default:
                                TempData["Status"] = "F";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Json(new { Status = TempData["Status"] }, JsonRequestBehavior.AllowGet);
        }
        [CheckSessions]
        [ValidateAntiForgeryToken]
        public ActionResult SaveExpert(ExpertsList obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@UserName", obj.UserName));
                    if (obj.IsActive != null && obj.IsActive == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@AreaofExpertise", obj.AreaofExpertise));
                    methodparameter.Add(new KeyValuePair<string, string>("@EmailID", obj.EmailID));
                    methodparameter.Add(new KeyValuePair<string, string>("@Mobile", obj.Mobile));
                    if (obj.UserID != null && obj.UserID != "" && obj.UserID != "0")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "U"));
                        methodparameter.Add(new KeyValuePair<string, string>("@UserID", obj.UserID));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "I"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID));
                    methodparameter.Add(new KeyValuePair<string, string>("@CreatedIP", BALCommon.GetIPAddress()));
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Expert_SaveExpert]", methodparameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            switch (res.Trim().ToUpper())
                            {
                                case "S":
                                    TempData["Status"] = "S";
                                    break;
                                case "E":
                                    TempData["Status"] = "O";
                                    TempData["Status_Type"] = BALCommon.CustomClientAlerts.AlertTypeFormat.Error;
                                    TempData["Status_Msg"] = ds.Tables[0].Rows[0]["msg"];
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("AddNewExpert");
        }
        //------------------------------Add Expert-----------------------------//
        //------------------------------Download Expert PDF-------------------------------------//
        public ActionResult SubjectExpertDownloadPDF()
        {
            Dictionary<string, string> LabelDictionary = FormLabels.GetFormLabels(FormLabels.Module.Masters,FormLabels.FormMasters.MasterExpertUsers, UserSession.LangCulture);
            Experts obj = new Experts();
            obj.List = new List<ExpertsList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetExpertList]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new ExpertsList
                            {
                                AreaofExpertise = Convert.ToString(dr["AreaofExpertise"]),
                                EmailID = Convert.ToString(dr["EmailID"]),
                                Mobile = Convert.ToString(dr["Mobile"]),
                                UserName = Convert.ToString(dr["UserName"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            ViewAsPdf PDF = new ViewAsPdf("Download/_DownloadPDFExpert", obj)
            {
                FileName = "_DownloadPDFExpert.pdf",
                PageMargins = { Left = 8, Right = 8 },
                CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
            byte[] PDFBytes = PDF.BuildPdf(ControllerContext);
            return File(PDFBytes, "application/pdf");
        }
        //------------------------------Download Expert PDF-------------------------------------//
        //------------------------------Download Expert Excel-------------------------------------//
        public ActionResult SubjectExpertDownloadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                }
                ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetExpertList]", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string[] DBColumnNames = new[] { "UserName", "Mobile", "EmailID", "AreaofExpertise" };
                    string[] NewColumnNames = new[] { "Expert Name", "Mobile No", "Email ID", "Area of Expertise" };
                    string[] ColumnsToRemoveNew = new[] { "UserID","IsActive" };

                    // Dim ColumnsToRemoveNew As String() = {}
                    System.Data.DataTable dtDataTable1 = BALCommon.UpdateExcelDataTable(ds.Tables[0], DBColumnNames, NewColumnNames, ColumnsToRemoveNew);

                    using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("ExpertList");
                        ws.Cell(1, 1).Value = "Expert List";
                        ws.Range(1, 1, 1, 4).Merge().AddToNamed("Titles");
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
                        Response.AddHeader("content-disposition", "attachment;filename=ExpertList.xlsx");

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
        //------------------------------Download Expert Excel-------------------------------------//
    }
}