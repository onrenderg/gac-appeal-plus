using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Masters.Data
{
    public class PublicMenuMaster
    {
        public List<PublicMenuMaster_Data> MenuList { get; set; }

        public static PublicMenuMaster GetMenus()
        {
            PublicMenuMaster PublicMenu = new PublicMenuMaster { MenuList = new List<PublicMenuMaster_Data>() };
            var methodParameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "FETCH"),
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    PublicMenu.MenuList.Add(new PublicMenuMaster_Data
                    {
                        MenuId = Convert.ToString(dr["menu_id"]),
                        LanguageCode = Convert.ToString(dr["LanguageCode"]),
                        LanguageDesc = Convert.ToString(dr["LanguageDesc"]),
                        MenuName = Convert.ToString(dr["menu_name"]),
                        MenuType = Convert.ToString(dr["menu_type"]),
                        AreaName = Convert.ToString(dr["area_name"]),
                        ControllerName = Convert.ToString(dr["controller_name"]),
                        ActionName = Convert.ToString(dr["action_nm"]),
                        MenuContent = Convert.ToString(dr["menu_content"]),
                        HyperLink = Convert.ToString(dr["hyperlink"]),
                        IsActive = Convert.ToString(dr["is_active"]),
                        DisplayOrder = Convert.ToString(dr["display_order"]),
                    });
                }
            }
            return PublicMenu;
        }

        public static List<SelectListItem> GetLanguageList(string MenuId, string LanguageCode)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            var methodParameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@MenuId", MenuId),
                new KeyValuePair<string, string>("@LanguageCode", LanguageCode),
                new KeyValuePair<string, string>("@Mode", "LANG_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new SelectListItem { Value = Convert.ToString(dr["languageCode"]), Text = Convert.ToString(dr["LanguageDesc"]) });
                }
            }
            return List;
        }
    }

    public class PublicMenuMaster_Data
    {
        public string qs { get; set; }
        public string Mode { get; set; }
        public string MenuId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string LanguageCode { get; set; }
        public string LanguageDesc { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^(?:C|A|H|P)$", ErrorMessage = "Invalid value!")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Invalid value!")]
        public string MenuType { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid characters!")]
        [StringLength(200, ErrorMessage = "Only 200 characters are allowed!")]
        public string AreaName { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid characters!")]
        [StringLength(200, ErrorMessage = "Only 200 characters are allowed!")]
        public string ControllerName { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid characters!")]
        [StringLength(200, ErrorMessage = "Only 200 characters are allowed!")]
        public string ActionName { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Mandatory Field!")]
        public string MenuContent { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string HyperLink { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^(?:Y|N)$", ErrorMessage = "Invalid value!")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Invalid value!")]
        public string IsActive { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed!")]
        [Range(0, 999, ErrorMessage = "Invalid value!")]
        public string DisplayOrder { get; set; }
        [Required(ErrorMessage = "Select .pdf only!")]
        public HttpPostedFileBase AttachmentFile { get; set; }
        public List<SelectListItem> LanguageList { get; set; }

        public static PublicMenuMaster_Data GetMenuData(string MenuId, string LanguageCode)
        {
            PublicMenuMaster_Data Menu = new PublicMenuMaster_Data { MenuId = MenuId, LanguageCode = LanguageCode };
            var methodParameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@MenuId", MenuId),
                new KeyValuePair<string, string>("@LanguageCode", LanguageCode),
                new KeyValuePair<string, string>("@Mode", "DETAILS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "PublicMenuManagement_Action", methodParameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Menu.MenuName = Convert.ToString(ds.Tables[0].Rows[0]["menu_name"]);
                Menu.MenuType = Convert.ToString(ds.Tables[0].Rows[0]["menu_type"]);
                Menu.AreaName = Convert.ToString(ds.Tables[0].Rows[0]["area_name"]);
                Menu.ControllerName = Convert.ToString(ds.Tables[0].Rows[0]["controller_name"]);
                Menu.ActionName = Convert.ToString(ds.Tables[0].Rows[0]["action_nm"]);
                Menu.HyperLink = Convert.ToString(ds.Tables[0].Rows[0]["hyperlink"]);
                Menu.MenuContent = Convert.ToString(ds.Tables[0].Rows[0]["menu_content"]);
                Menu.IsActive = Convert.ToString(ds.Tables[0].Rows[0]["is_active"]);
                Menu.DisplayOrder = Convert.ToString(ds.Tables[0].Rows[0]["display_order"]);
            }
            return Menu;
        }
    }
}