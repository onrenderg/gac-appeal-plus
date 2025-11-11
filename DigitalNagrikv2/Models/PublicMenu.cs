using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class PublicMenu
    {
        private static readonly string URLEmpty = "javascript:void(0)";
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuType { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string MenuContent { get; set; }
        public string HyperLink { get; set; }

        private static List<PublicMenu> GetAllMenuNodes()
        {
            List<PublicMenu> Nodes = new List<PublicMenu>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@LanguageCode", UserSession.LangCulture));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Fetch_PublicMenu", methodParameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Nodes.Add(new PublicMenu
                    {
                        MenuId = Convert.ToString(dr["menu_id"]),
                        MenuName = Convert.ToString(dr["menu_name"]),
                        MenuType = Convert.ToString(dr["menu_type"]),
                        AreaName = Convert.ToString(dr["area_name"]),
                        ControllerName = Convert.ToString(dr["controller_name"]),
                        ActionName = Convert.ToString(dr["action_nm"]),
                        HyperLink = Convert.ToString(dr["hyperlink"]),
                    });
                }
            }
            return Nodes;
        }

        public static MvcHtmlString RenderMenu()
        {
            StringBuilder MenuHtmlString = new StringBuilder();
            List<PublicMenu> Nodes = GetAllMenuNodes();
            if (Nodes.Count() > 0)
            {
                int NodeCounter = 1;
                foreach (PublicMenu Node in Nodes)
                {
                    MenuHtmlString.Append(GenerateMenuLink(Node)); if (NodeCounter < Nodes.Count()) { MenuHtmlString.Append("<span class='mx-1' style='font-weight:normal'>|</span>"); }
                    ++NodeCounter;
                }
            }
            else
                MenuHtmlString.Append("<div class=''>Failure retrieving data<div>");
            return MvcHtmlString.Create(MenuHtmlString.ToString());
        }

        private static string GenerateMenuLink(PublicMenu Node)
        {
            StringBuilder Link = new StringBuilder();
            var URLHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string EncQs = SecureQueryString.SecureQueryString.Encrypt("MenuId=" + Node.MenuId);
            if (Node.MenuType == "A")
            {
                string URL = URLHelper.Action(Node.ActionName, Node.ControllerName, new { Area = Node.AreaName });
                Link.AppendFormat("<a class='{0}' href='{1}'>{2}</a>", "", string.IsNullOrWhiteSpace(URL) ? URLEmpty : URL + "?qs=" + EncQs, Node.MenuName);
            }
            else if (Node.MenuType == "C")
            {
                string URL = URLHelper.Action("CMSContent", "CMSData", new { Area = "" });
                Link.AppendFormat("<a class='{0}' href='{1}'>{2}</a>", "", URL + "?qs=" + EncQs, Node.MenuName);
            }
            else if (Node.MenuType == "P")
            {
                string URL = URLHelper.Action("CMSViewAttachment", "CMSData", new { Area = "" });
                Link.AppendFormat("<a class='{0}' target='_blank' rel='noopener noreferrer' href='{1}'>{2}</a>", "", URL + "?qs=" + EncQs, Node.MenuName);
            }
            else if (Node.MenuType == "J")
            {
                string URL = "#";
                string hyperlink = Node.HyperLink;
                if (Node.ActionName != "#")
                {
                    URL = URLHelper.Action(Node.ActionName, Node.ControllerName, new { Area = Node.AreaName });
                }

                Link.AppendFormat("<a class='{0}'  href='{1}' onclick='" + hyperlink + "'>{2}</a>", "", URL, Node.MenuName);
            }
            return Link.ToString();
        }

        public static PublicMenu GetmenuContent(string qs)
        {
            var Params = QueryString.GetDecryptedParameters(qs);
            PublicMenu MenuContent = new PublicMenu();
            try
            {
                string MenuId = Convert.ToString(Params["MenuId"]); MenuContent.MenuId = MenuId;
                var methodParameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@MenuId", MenuId),
                    new KeyValuePair<string, string>("@LanguageCode", UserSession.LangCulture),
                    new KeyValuePair<string, string>("@Mode", "CONTENT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    MenuContent.MenuName = Convert.ToString(ds.Tables[0].Rows[0]["menu_name"]);
                    MenuContent.MenuContent = Convert.ToString(ds.Tables[0].Rows[0]["menu_content"]);
                }
            }
            catch (Exception ex) { }
            return MenuContent;
        }
    }
}