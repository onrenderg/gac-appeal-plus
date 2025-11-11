using DigitalNagrik.Areas.Masters.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Areas.Masters.Controllers
{
    public class PublicMenuMasterController : Controller
    {
        // GET: Masters/PublicMenuMaster

        [CheckSessions]
        public ActionResult Index()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACDashboard, "1", UserSession.LangCulture);
            PublicMenuMaster Menu = PublicMenuMaster.GetMenus();
            return View(Menu);
        }

        [CheckSessions]
        public ActionResult AddMenuForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            var Params = QueryString.GetDecryptedParameters(qs); string MenuId = string.Empty; string LanguageCode = string.Empty; try { MenuId = Convert.ToString(Params["MenuId"]); LanguageCode = Convert.ToString(Params["LanguageCode"]); } catch { }
            PublicMenuMaster_Data Menu = PublicMenuMaster_Data.GetMenuData(MenuId, LanguageCode); Menu.qs = qs;
            return PartialView("_AddNewMenu", Menu);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(PublicMenuMaster_Data PostData)
        {
            TempData["Status"] = "F";
            ModelState.Remove("LanguageCode");
            if (PostData.MenuType == "C") { ModelState.Remove("AttachmentFile"); ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("HyperLink"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.HyperLink = ""; }
            else if (PostData.MenuType == "A") { ModelState.Remove("AttachmentFile"); ModelState.Remove("MenuContent"); ModelState.Remove("HyperLink"); PostData.MenuContent = ""; PostData.HyperLink = ""; }
            else if (PostData.MenuType == "H") { ModelState.Remove("AttachmentFile"); ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("MenuContent"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.MenuContent = ""; }
            else if (PostData.MenuType == "P") { ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("MenuContent"); ModelState.Remove("HyperLink"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.MenuContent = ""; PostData.HyperLink = ""; }

            //if (PostData.MenuType == "C") { ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("HyperLink"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.HyperLink = ""; }
            //else if (PostData.MenuType == "A") { ModelState.Remove("MenuContent"); ModelState.Remove("HyperLink"); PostData.MenuContent = ""; PostData.HyperLink = ""; }
            //else if (PostData.MenuType == "H") { ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("MenuContent"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.MenuContent = ""; }
            if (ModelState.IsValid)
            {
                string BytesDocument = "";
                if (PostData.AttachmentFile != null)
                {
                    byte[] EXE_Document = { 77, 90 };
                    byte[] fileBytes = new byte[PostData.AttachmentFile.ContentLength];
                    _ = PostData.AttachmentFile.InputStream.Read(fileBytes, 0, PostData.AttachmentFile.ContentLength);
                    if (PostData.AttachmentFile.ContentLength > 1000000)
                    {
                        TempData["Status"] = "F"; //"File size should be less than 1024 kb.";
                                                  //return RedirectToAction("Index");
                    }
                    if (fileBytes[0] == EXE_Document[0] && fileBytes[1] == EXE_Document[1])
                    {
                        TempData["StatusMessage"] = "N";//"File type EXE not supported.";
                                                        //return RedirectToAction("Index");
                    }
                    BytesDocument = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                }
                var methodParameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@MenuName", PostData.MenuName.Trim()),
                    new KeyValuePair<string, string>("@MenuType", PostData.MenuType.Trim()),
                    new KeyValuePair<string, string>("@AreaName", Trim(PostData.AreaName)),
                    new KeyValuePair<string, string>("@ControllerName", Trim(PostData.ControllerName)),
                    new KeyValuePair<string, string>("@ActionName", Trim(PostData.ActionName)),
                    new KeyValuePair<string, string>("@MenuContent", Trim(PostData.MenuContent)),
                    new KeyValuePair<string, string>("@HyperLink", Trim(PostData.HyperLink)),
                    new KeyValuePair<string, string>("@IsActive", PostData.IsActive),
                    new KeyValuePair<string, string>("@DisplayOrder", PostData.DisplayOrder),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()),
                    new KeyValuePair<string, string>("@AttachmentFile", Convert.ToString(BytesDocument)),
                    new KeyValuePair<string, string>("@Mode", "ADD")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]).Trim();
                    switch (res.ToUpper())
                    {
                        case "S":
                            TempData["Status"] = "S";
                            break;
                        case "NE":
                            TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, "Action Failed", "Menu with same name already exists.");
                            break;
                    }
                }
            }
            return RedirectToAction("Index", "PublicMenuMaster", new { area = "Masters" });
        }

        [CheckSessions]
        public ActionResult AddUpdateLanguageForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            var Params = QueryString.GetDecryptedParameters(qs); string MenuId = string.Empty; string LanguageCode = string.Empty; string Mode = string.Empty; try { MenuId = Convert.ToString(Params["MenuId"]); LanguageCode = Convert.ToString(Params["LanguageCode"]); Mode = Convert.ToString(Params["Mode"]); } catch { }
            PublicMenuMaster_Data Menu = PublicMenuMaster_Data.GetMenuData(MenuId, LanguageCode); Menu.qs = qs; Menu.Mode = Mode;
            Menu.LanguageList = PublicMenuMaster.GetLanguageList(MenuId, LanguageCode);
            if (!string.IsNullOrWhiteSpace(LanguageCode)) { Menu.LanguageList.RemoveAll(x => x.Value != LanguageCode); } else { }
            return PartialView("_AddUpdateLanguage", Menu);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateLanguage(PublicMenuMaster_Data PostData)
        {
            TempData["Status"] = "F";
            //if (PostData.MenuType == "C") { ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("HyperLink"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.HyperLink = ""; }
            //else if (PostData.MenuType == "A") { ModelState.Remove("MenuContent"); ModelState.Remove("HyperLink"); PostData.MenuContent = ""; PostData.HyperLink = ""; }
            //else if (PostData.MenuType == "H") { ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("MenuContent"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.MenuContent = ""; }
            if (PostData.MenuType == "C") { ModelState.Remove("AttachmentFile"); ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("HyperLink"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.HyperLink = ""; }
            else if (PostData.MenuType == "A") { ModelState.Remove("AttachmentFile"); ModelState.Remove("MenuContent"); ModelState.Remove("HyperLink"); PostData.MenuContent = ""; PostData.HyperLink = ""; }
            else if (PostData.MenuType == "H") { ModelState.Remove("AttachmentFile"); ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("MenuContent"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.MenuContent = ""; }
            else if (PostData.MenuType == "P") { ModelState.Remove("Hyperlink"); ModelState.Remove("AreaName"); ModelState.Remove("ControllerName"); ModelState.Remove("ActionName"); ModelState.Remove("MenuContent"); PostData.ActionName = ""; PostData.ControllerName = ""; PostData.AreaName = ""; PostData.MenuContent = ""; }

            var Params = QueryString.GetDecryptedParameters(PostData.qs); string MenuId = string.Empty; string Mode = string.Empty; try { MenuId = Convert.ToString(Params["MenuId"]); Mode = Convert.ToString(Params["Mode"]); } catch { }
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(MenuId) && !string.IsNullOrWhiteSpace(PostData.LanguageCode) && !string.IsNullOrWhiteSpace(Mode))
            {
                string BytesDocument = "";
                if (PostData.AttachmentFile != null)
                {
                    byte[] EXE_Document = { 77, 90 };
                    byte[] fileBytes = new byte[PostData.AttachmentFile.ContentLength];
                    _ = PostData.AttachmentFile.InputStream.Read(fileBytes, 0, PostData.AttachmentFile.ContentLength);
                    if (PostData.AttachmentFile.ContentLength > 1000000)
                    {
                        TempData["Status"] = "F"; //"File size should be less than 1024 kb.";
                                                  //return RedirectToAction("Index");
                    }
                    if (fileBytes[0] == EXE_Document[0] && fileBytes[1] == EXE_Document[1])
                    {
                        TempData["StatusMessage"] = "N";//"File type EXE not supported.";
                                                        //return RedirectToAction("Index");
                    }
                    BytesDocument = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                }
                var methodParameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@MenuId", MenuId),
                    new KeyValuePair<string, string>("@LanguageCode", PostData.LanguageCode.Trim()),
                    new KeyValuePair<string, string>("@MenuName", PostData.MenuName.Trim()),
                    new KeyValuePair<string, string>("@MenuType", PostData.MenuType.Trim()),
                    new KeyValuePair<string, string>("@AreaName", Trim(PostData.AreaName)),
                    new KeyValuePair<string, string>("@ControllerName", Trim(PostData.ControllerName)),
                    new KeyValuePair<string, string>("@ActionName", Trim(PostData.ActionName)),
                    new KeyValuePair<string, string>("@MenuContent", Trim(PostData.MenuContent)),
                    new KeyValuePair<string, string>("@HyperLink", Trim(PostData.HyperLink)),
                    new KeyValuePair<string, string>("@IsActive", PostData.IsActive),
                    new KeyValuePair<string, string>("@DisplayOrder", PostData.DisplayOrder),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()),
                    new KeyValuePair<string, string>("@AttachmentFile", Convert.ToString(BytesDocument)),
                    new KeyValuePair<string, string>("@Mode", Mode)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]).Trim();
                    switch (res.ToUpper())
                    {
                        case "S":
                            TempData["Status"] = "S";
                            break;
                    }
                }
            }
            return RedirectToAction("Index", "PublicMenuMaster", new { area = "Masters" });
        }

        [CheckSessions]
        public ActionResult DeleteMenu(string qs)
        {
            TempData["Status"] = "F";
            var Params = QueryString.GetDecryptedParameters(qs); string MenuId = string.Empty; string LanguageCode = string.Empty; string Mode = string.Empty; try { MenuId = Convert.ToString(Params["MenuId"]); LanguageCode = Convert.ToString(Params["LanguageCode"]); Mode = Convert.ToString(Params["Mode"]); } catch { }

            if (!string.IsNullOrWhiteSpace(MenuId) && !string.IsNullOrWhiteSpace(LanguageCode) && !string.IsNullOrWhiteSpace(Mode))
            {
                var methodParameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@MenuId", MenuId),
                    new KeyValuePair<string, string>("@LanguageCode", LanguageCode),
                    new KeyValuePair<string, string>("@Mode", Mode)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]).Trim();
                    switch (res.ToUpper())
                    {
                        case "S":
                            TempData["Status"] = "D";
                            break;
                    }
                }
            }
            return RedirectToAction("Index", "PublicMenuMaster", new { area = "Masters" });
        }
    }
}