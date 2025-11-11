using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace DigitalNagrik.Controllers
{
    public class CMSDataController : Controller
    {
        // GET: CMSData
        public ActionResult Index()
        {
            return View();
        }
        //Public

        public ActionResult CMSContent(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            PublicMenu Menu = PublicMenu.GetmenuContent(qs);
            return View(Menu);
        }
        public ActionResult CMSViewAttachment(string qs)
        {
            if (qs != "")
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
                byte[] DocumentView = new Byte[] { };
                var Params = QueryString.GetDecryptedParameters(qs);
                string MenuId = Convert.ToString(Params["MenuId"]);
                var methodParameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@MenuId", MenuId),
                    new KeyValuePair<string, string>("@Mode", "Attachment")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DocumentView = (ds.Tables[0].Rows[0]["AttachmentFile"]) == DBNull.Value ? null : (byte[])(ds.Tables[0].Rows[0]["AttachmentFile"]);

                }

                //return View(obj);
                return File(DocumentView, "application/pdf");
            }
            return new EmptyResult();

        }

        public ActionResult GetBreadcrumbs(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            PublicMenu Menu = PublicMenu.GetmenuContent(qs);
            return PartialView("_Breadcrumbs", Menu);
        }

        public ActionResult Documents(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            PublicMenu Menu = PublicMenu.GetmenuContent(qs); ViewBag.Menu = Menu;
            CMSDataAddDocument obj = new CMSDataAddDocument();
            try
            {
                obj.DocumnetListTable = GetDocumnetListTable_List("0");
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        public ActionResult Helpline()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            Helpline obj = new Helpline();
            try
            {
                obj.HelpSectionList.Add(new SelectListItem { Text = "Web Portal", Value = "WEB" });
                obj.HelpSectionList.Add(new SelectListItem { Text = "Mobile App", Value = "MOB" });
                obj.ContentTypeList.Add(new SelectListItem { Text = "Documents", Value = "DOCUMENT" });
                obj.ContentTypeList.Add(new SelectListItem { Text = "Web Links", Value = "LINK" });
                obj.ContentTypeList.Add(new SelectListItem { Text = "Videos", Value = "VIDEO" });
                obj.DocumnetListTable = GetHelplineListPublic_List("0", "0", "Y");
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        public List<Helpline.DocumnetListTable_List> GetHelplineListPublic_List(string ContentType, string DeviceNm, string Enable)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            List<Helpline.DocumnetListTable_List> Data = new List<Helpline.DocumnetListTable_List>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    methodparameter.Add(new KeyValuePair<string, string>("@DeviceNm", DeviceNm));
                    methodparameter.Add(new KeyValuePair<string, string>("@ContentType", ContentType));
                    methodparameter.Add(new KeyValuePair<string, string>("@Enable", Enable));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_GetHelpLineData", methodparameter);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.Add(new Helpline.DocumnetListTable_List
                        {
                            Name = Convert.ToString(dr["Name"]),
                            ContentType = Convert.ToString(dr["ContentType"]),
                            URL = Convert.ToString(dr["URL"]),
                            DocID = Convert.ToString(dr["DocID"]),
                            DeviceName = Convert.ToString(dr["DeviceName"]),
                            HelpLineID = Convert.ToString(dr["HelpLineID"]),
                            OrderofAppearance = Convert.ToString(dr["OrderofAppearance"])
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Data;
        }
        public ActionResult FAQs(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            PublicMenu Menu = PublicMenu.GetmenuContent(qs); ViewBag.Menu = Menu;
            try
            {
                Models.FAQs obj = new Models.FAQs();
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    methodparameter.Add(new KeyValuePair<string, string>("@LanguageCode", UserSession.LangCulture));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_getFAQs", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        List<FAQ> listFAQ = new List<FAQ>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            listFAQ.Add(new Models.FAQ(Convert.ToInt32(dr["SrNo"].ToString()), dr["Question"].ToString(), dr["Answer"].ToString(), dr["Hyperlink"].ToString()));
                        }
                        obj.FAQList = listFAQ;
                    }
                }
                return View(obj);
            }
            catch (Exception)
            {

                _ = RedirectToAction("Error", "Home");
            }
            return View();

        }
        //Public
        //Add FAQs
        [CheckSessions]
        public ActionResult AddFAQs()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            CMSDataAddFAQs obj = new CMSDataAddFAQs();
            try
            {
                obj.QAListTable = GetQAListTable();
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        [CheckSessions]
        public List<QAListTable_List> GetQAListTable()
        {
            List<QAListTable_List> Data = new List<QAListTable_List>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetCMS_FAQList", methodparameter);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.Add(new QAListTable_List
                        {
                            Question = Convert.ToString(dr["Question"]),
                            Answer = Convert.ToString(dr["Answer"]),
                            DisplayOrder = Convert.ToString(dr["DisplayOrder"]),
                            FAQID = Convert.ToString(dr["FAQID"]),
                            isActive = Convert.ToString(dr["isActive"]),
                            Hyperlink = Convert.ToString(dr["Hyperlink"])
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Data;
        }
        [CheckSessions]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult InsertUpdFAQs(CMSDataAddFAQs obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@Question", obj.Question),
                        new KeyValuePair<string, string>("@Answer", obj.Answer),
                        new KeyValuePair<string, string>("@isActive", obj.isActive),
                        new KeyValuePair<string, string>("@DisplayOrder", obj.DisplayOrder),
                        new KeyValuePair<string, string>("@Hyperlink", obj.Hyperlink),
                        new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID),
                        new KeyValuePair<string, string>("@CreationIP", CommonRepository.GetIPAddress())
                    };
                    if (obj.FAQID != null && obj.FAQID != "")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Func", "UPDATE"));
                        methodparameter.Add(new KeyValuePair<string, string>("@FAQID", obj.FAQID));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Func", "INSERT"));
                    }

                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_InsertUpdFAQs", methodparameter);
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
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("AddFAQs");
        }
        [CheckSessions]
        public ActionResult EditFAQs(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            try
            {
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string FAQID = Params["FAQID"];
                CMSDataAddFAQs Data = new CMSDataAddFAQs();
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@FAQID", FAQID)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetCMS_FAQList", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Data.Question = Convert.ToString(ds.Tables[0].Rows[0]["Question"]);
                        Data.Answer = Convert.ToString(ds.Tables[0].Rows[0]["Answer"]);
                        Data.DisplayOrder = Convert.ToString(ds.Tables[0].Rows[0]["DisplayOrder"]);
                        Data.isActive = Convert.ToString(ds.Tables[0].Rows[0]["isActive"]);
                        Data.Hyperlink = Convert.ToString(ds.Tables[0].Rows[0]["Hyperlink"]);
                        Data.FAQID = Convert.ToString(ds.Tables[0].Rows[0]["FAQID"]);
                        Data.QAListTable = GetQAListTable();
                    }
                }
                return View("AddFAQs", Data);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("index");
        }
        [CheckSessions]
        public ActionResult DeleteFAQs(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            try
            {
                TempData["Status"] = "F";
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string FAQID = Params["FAQID"];
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@FAQID", FAQID),
                    new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID),
                    new KeyValuePair<string, string>("@CreationIP", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@func", "DELETE")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_InsertUpdFAQs", methodparameter);
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
            return RedirectToAction("AddFAQs");
        }
        //AddFAQs
        //AddDocument
        [CheckSessions]
        public ActionResult AddDocument()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            CMSDataAddDocument obj = new CMSDataAddDocument();
            try
            {
                obj = GetMasterData();
                obj.DocumnetListTable = GetDocumnetListTable_List("1");
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        [CheckSessions]
        private CMSDataAddDocument GetMasterData()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            CMSDataAddDocument Data = new CMSDataAddDocument();
            try
            {
                Data.DocumentSortByList.Add(new SelectListItem { Text = "Sort By Doc/ Entry Date (Desc)", Value = "DD" });
                Data.DocumentSortByList.Add(new SelectListItem { Text = "Sort By Doc/ Entry Date (Asc)", Value = "CS" });

                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {

                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetDocumentSection", methodparameter);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.DocumentSelectionList.Add(new SelectListItem { Text = Convert.ToString(dr["DocSectionName"]), Value = Convert.ToString(dr["DocSectionID"]) });
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return Data;
        }
        [CheckSessions]
        public List<CMSDataAddDocument.DocumnetListTable_List> GetDocumnetListTable_List(string DocSectionID)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            List<CMSDataAddDocument.DocumnetListTable_List> Data = new List<CMSDataAddDocument.DocumnetListTable_List>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    methodparameter.Add(new KeyValuePair<string, string>("@LanguageCode", UserSession.LangCulture));
                    methodparameter.Add(new KeyValuePair<string, string>("@DocSectionID", DocSectionID));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_DocumentList_getall", methodparameter);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.Add(new CMSDataAddDocument.DocumnetListTable_List
                        {
                            DocID = Convert.ToString(dr["DocID"]),
                            DocumentName_link = Convert.ToString(dr["DocOrLinkName"]),
                            DocumentType = Convert.ToString(dr["DocType"]),
                            inNewTab = Convert.ToString(dr["InNewTab"]),
                            IsActive = Convert.ToString(dr["isActive"]),
                            PDFlink_URLLink = Convert.ToString(dr["FileOrLinkURL"]),
                            ShownewInfographics = Convert.ToString(dr["Infographic"]),
                            Validity = Convert.ToString(dr["Validity"]),
                            DocSectionID = Convert.ToString(dr["DocSectionID"]),
                            DocOrLinkDate = Convert.ToString(dr["DocOrLinkDate"]),
                            DisplayPref = Convert.ToString(dr["Validity"])


                        });
                    }
                }

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Data;
        }
        [CheckSessions]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult InsertUpdDocument(CMSDataAddDocument obj)
        {
            try
            {
                if (obj.ContentType == "DOCUMENT")
                {
                    _ = ModelState.Remove("LinkAddress");

                }
                else if (obj.ContentType == "LINK")
                {
                    _ = ModelState.Remove("DocumenttoUpload");
                    _ = ModelState.Remove("DocumentDate");

                }

                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    string BytesDocument = "";
                    if (obj.DocumenttoUpload != null)
                    {
                        byte[] EXE_Document = { 77, 90 };
                        byte[] fileBytes = new byte[obj.DocumenttoUpload.ContentLength];
                        _ = obj.DocumenttoUpload.InputStream.Read(fileBytes, 0, obj.DocumenttoUpload.ContentLength);
                        if (obj.DocumenttoUpload.ContentLength > 1000000)
                        {
                            TempData["Status"] = "F"; //"File size should be less than 1024 kb.";
                            //return RedirectToAction("Index");
                        }
                        if (fileBytes[0] == EXE_Document[0] && fileBytes[1] == EXE_Document[1])
                        {
                            TempData["Status"] = "N";//"File type EXE not supported.";
                            //return RedirectToAction("Index");
                        }
                        BytesDocument = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                    }

                    List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@DocID", obj.DocID),
                        new KeyValuePair<string, string>("@DocSectionID", obj.DocumentSelectionID),
                        new KeyValuePair<string, string>("@DocType", obj.ContentType),
                        new KeyValuePair<string, string>("@DocOrLinkName", obj.DocumentName),
                        new KeyValuePair<string, string>("@PDF", BytesDocument),

                        new KeyValuePair<string, string>("@Validity", obj.DocumentValidity),
                        new KeyValuePair<string, string>("@DocOrLinkDate", obj.DocumentDate),
                        new KeyValuePair<string, string>("@Infographic", obj.ShowNewInfographics),
                        new KeyValuePair<string, string>("@FileOrLinkURL", obj.LinkAddress),
                        new KeyValuePair<string, string>("@InNewTab", obj.OpeninNewTab),

                        new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID),
                        new KeyValuePair<string, string>("@CreatedIP", CommonRepository.GetIPAddress()),
                        new KeyValuePair<string, string>("@DispPref", obj.DisplayPref)
                    };


                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_InsertUpdDocument", methodparameter);
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
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("AddDocument");
        }
        [CheckSessions]
        public ActionResult EditDocument(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            try
            {
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string FAQID = Params["FAQID"];
                CMSDataAddFAQs Data = new CMSDataAddFAQs();
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@FAQID", FAQID)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetCMS_FAQList", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Data.Question = Convert.ToString(ds.Tables[0].Rows[0]["Question"]);
                        Data.Answer = Convert.ToString(ds.Tables[0].Rows[0]["Answer"]);
                        Data.DisplayOrder = Convert.ToString(ds.Tables[0].Rows[0]["DisplayOrder"]);
                        Data.isActive = Convert.ToString(ds.Tables[0].Rows[0]["isActive"]);
                        Data.Hyperlink = Convert.ToString(ds.Tables[0].Rows[0]["Hyperlink"]);
                        Data.FAQID = Convert.ToString(ds.Tables[0].Rows[0]["FAQID"]);
                        Data.QAListTable = GetQAListTable();
                    }
                }
                return View("AddFAQs", Data);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("index");
        }
        [CheckSessions]
        public ActionResult DeleteDocument(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            try
            {
                TempData["Status"] = "F";
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string DocID = Params["DocID"];
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@DocID", DocID),
                    new KeyValuePair<string, string>("@Command", "Delete")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_DocumentList_Commands", methodparameter);
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
            return RedirectToAction("AddDocument");
        }
        public ActionResult ViewDocumentPDF(string qs)
        {
            try
            {
                if (qs != "")
                {
                    byte[] DocumentView = new byte[] { };
                    Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                    string DocID = Params["DocID"];
                    List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@DocID", DocID)
                    };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_DocumentList_getDocument", methodparameter);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DocumentView = ds.Tables[0].Rows[0]["PDF"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["PDF"];
                            }
                        }
                    }
                    //return View(obj);
                    return File(DocumentView, "application/pdf");
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return new EmptyResult();
        }
        //AddDocument
        //DropDownChange
        public ActionResult ChangeDocumentListData(string DocSectionID)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            CMSDataAddDocument cMSDataAddDocument = new CMSDataAddDocument();
            try
            {
                cMSDataAddDocument.DocumnetListTable = GetDocumnetListTable_List(DocSectionID);
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return PartialView("_DocumentLinkList", cMSDataAddDocument);
        }
        //DropDown Change
        //Switches
        [CheckSessions]
        public JsonResult Switches(string DocID, string Command)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@DocID", DocID),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_DocumentList_Commands", methodparameter);
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
        //Switches
        //AddHelplineData
        [CheckSessions]
        public ActionResult AddHelpline()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            CMSDatatAddHelpline obj = new CMSDatatAddHelpline();
            try
            {
                obj = GetMasterDataHelpline();
                obj.DocumnetListTable = GetHelplineListTable_List("0", "0");
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        [CheckSessions]
        private CMSDatatAddHelpline GetMasterDataHelpline()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CMSDatatAddHelpline Data = new CMSDatatAddHelpline();
            try
            {
                Data.DeviceNameList.Add(new SelectListItem { Text = "Web Portal", Value = "WEB" });
                Data.DeviceNameList.Add(new SelectListItem { Text = "Mobile App", Value = "MOB" });
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return Data;
        }
        [CheckSessions]
        public List<CMSDatatAddHelpline.DocumnetListTable_List> GetHelplineListTable_List(string ContentType, string DeviceNm)
        {
            List<CMSDatatAddHelpline.DocumnetListTable_List> Data = new List<CMSDatatAddHelpline.DocumnetListTable_List>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    methodparameter.Add(new KeyValuePair<string, string>("@DeviceNm", DeviceNm));
                    methodparameter.Add(new KeyValuePair<string, string>("@ContentType", ContentType));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_GetHelpLineData", methodparameter);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.Add(new CMSDatatAddHelpline.DocumnetListTable_List
                        {
                            HelpLineID = Convert.ToString(dr["HelpLineID"]),
                            DocumentName_link = Convert.ToString(dr["Name"]),
                            DocumentType = Convert.ToString(dr["ContentType"]),
                            DeviceName = Convert.ToString(dr["DeviceName"]),
                            IsActive = Convert.ToString(dr["Enable"]),
                            PDFlink_URLLink = Convert.ToString(dr["URL"]),
                            DisplayOrder = Convert.ToString(dr["OrderofAppearance"]),
                            DocID = Convert.ToString(dr["DocID"])
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Data;
        }
        [CheckSessions]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult InsertUpdHelpline(CMSDatatAddHelpline obj)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            try
            {
                if (obj.ContentType == "DOCUMENT")
                {
                    _ = ModelState.Remove("LinkAddress");
                }
                else if (obj.ContentType == "LINK")
                {
                    _ = ModelState.Remove("DocumenttoUpload");
                }
                else if (obj.ContentType == "VIDEO")
                {
                    _ = ModelState.Remove("DocumenttoUpload");
                }

                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    string BytesDocument = "";
                    if (obj.DocumenttoUpload != null)
                    {
                        byte[] EXE_Document = { 77, 90 };
                        byte[] fileBytes = new byte[obj.DocumenttoUpload.ContentLength];
                        _ = obj.DocumenttoUpload.InputStream.Read(fileBytes, 0, obj.DocumenttoUpload.ContentLength);
                        if (obj.DocumenttoUpload.ContentLength > 1000000)
                        {
                            TempData["Status"] = "F"; //"File size should be less than 1024 kb.";
                            //return RedirectToAction("Index");
                        }
                        if (fileBytes[0] == EXE_Document[0] && fileBytes[1] == EXE_Document[1])
                        {
                            TempData["Status"] = "N";//"File type EXE not supported.";
                            //return RedirectToAction("Index");
                        }
                        BytesDocument = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                    }

                    List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@HelplineID", obj.HelplineID),
                        new KeyValuePair<string, string>("@Action", "S"),
                        new KeyValuePair<string, string>("@DeviceName", obj.DeviceNameID),
                        new KeyValuePair<string, string>("@ContentType", obj.ContentType),
                        new KeyValuePair<string, string>("@Name", obj.DocumentName),
                        new KeyValuePair<string, string>("@OrderofAppearance", obj.DisplayOrder),
                        new KeyValuePair<string, string>("@URL", obj.LinkAddress),
                        new KeyValuePair<string, string>("@EnableDisable", obj.Enabled),
                        new KeyValuePair<string, string>("@DocID", obj.DocID),
                        new KeyValuePair<string, string>("@DocFile", BytesDocument),
                        new KeyValuePair<string, string>("@DocType", "PDF"),
                        new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID),
                        new KeyValuePair<string, string>("@CreatedIP", CommonRepository.GetIPAddress())
                    };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_HelpLineData_insupd", methodparameter);
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
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("AddHelpline");
        }
        [CheckSessions]
        public ActionResult EditHelpline(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            try
            {
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string FAQID = Params["FAQID"];
                CMSDataAddFAQs Data = new CMSDataAddFAQs();
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@FAQID", FAQID)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetCMS_FAQList", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Data.Question = Convert.ToString(ds.Tables[0].Rows[0]["Question"]);
                        Data.Answer = Convert.ToString(ds.Tables[0].Rows[0]["Answer"]);
                        Data.DisplayOrder = Convert.ToString(ds.Tables[0].Rows[0]["DisplayOrder"]);
                        Data.isActive = Convert.ToString(ds.Tables[0].Rows[0]["isActive"]);
                        Data.Hyperlink = Convert.ToString(ds.Tables[0].Rows[0]["Hyperlink"]);
                        Data.FAQID = Convert.ToString(ds.Tables[0].Rows[0]["FAQID"]);
                        Data.QAListTable = GetQAListTable();
                    }
                }
                return View("AddFAQs", Data);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("index");
        }
        [CheckSessions]
        public ActionResult DeleteHelpline(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            try
            {
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string HelplineID = Params["HelplineID"];
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@HelplineID", HelplineID),
                    new KeyValuePair<string, string>("@Command", "Delete")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_HelplineData_Commands", methodparameter);
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
            return RedirectToAction("AddHelpline");
        }
        //[CheckSessions]
        public ActionResult ViewHelplinePDF(string qs)
        {
            try
            {
                if (qs != "")
                {
                    byte[] DocumentView = new byte[] { };
                    Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                    string DocID = Params["DocID"];
                    List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@DocID", DocID)
                    };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_HelpLine_getImage", methodparameter);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DocumentView = ds.Tables[0].Rows[0]["DocFile"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["DocFile"];
                            }
                        }
                    }
                    //return View(obj);
                    return File(DocumentView, "application/pdf");
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return new EmptyResult();
        }
        [CheckSessions]
        public JsonResult SwitchesHelpline(string HelplineID, string Command)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@HelplineID", HelplineID),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CMS_HelplineData_Commands", methodparameter);
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
        //Add Helpline Data
    }
}