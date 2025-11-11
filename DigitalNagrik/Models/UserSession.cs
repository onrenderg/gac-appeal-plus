using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class UserRoleType
    {
        public const string Admin = "1";
        public const string Secretariat = "2";
        public const string GAC = "3";
    }
    public class UserRoles
    {
        public const string Admin = "1";
        public const string Secretariat = "2";
        public const string Chairperson = "5";
        public const string MemberSecretary = "6";
        public const string Member = "7";
        public const string FrontDesk_DealingHand = "9";
        public const string GACManager = "10";
        //public const string GACAssistantManager = "11";
    }

    public class IntermediaryForSession
    {
        public string IntermediaryTitle = string.Empty;
        public string UserName = string.Empty;
        public  string GOEmail = string.Empty;
        public  string Designation = string.Empty;
        public  string URL = string.Empty;
        public  string HelpLink = string.Empty;
        public  string Address = string.Empty;

    }

    public class UserSession
    {
        public static string isSSOLogin
        {
            get { return ((HttpContext.Current.Session["IsSSOLogin"] == null) ? string.Empty : (string)HttpContext.Current.Session["IsSSOLogin"]).Trim(); }
            set { HttpContext.Current.Session["IsSSOLogin"] = value; }
        }
        public static string UserID
        {
            get { return ((HttpContext.Current.Session["UserID"] == null) ? string.Empty : (string)HttpContext.Current.Session["UserID"]).Trim(); }
            set { HttpContext.Current.Session["UserID"] = value; }
        }

        public static string UserName
        {
            get { return ((HttpContext.Current.Session["UserName"] == null) ? string.Empty : (string)HttpContext.Current.Session["UserName"]).Trim(); }
            set { HttpContext.Current.Session["UserName"] = value; }
        }

        public static string EmailId
        {
            get { return ((HttpContext.Current.Session["EmailId"] == null) ? string.Empty : (string)HttpContext.Current.Session["EmailId"]).Trim(); }
            set { HttpContext.Current.Session["EmailId"] = value; }
        }

        public static string Mobile
        {
            get { return ((HttpContext.Current.Session["Mobile"] == null) ? string.Empty : (string)HttpContext.Current.Session["Mobile"]).Trim(); }
            set { HttpContext.Current.Session["Mobile"] = value; }
        }
        public static string Designation
        {
            get { return ((HttpContext.Current.Session["Designation"] == null) ? string.Empty : (string)HttpContext.Current.Session["Designation"]).Trim(); }
            set { HttpContext.Current.Session["Designation"] = value; }
        }
        public static string RoleID
        {
            get { return ((HttpContext.Current.Session["RoleID"] == null) ? string.Empty : (string)HttpContext.Current.Session["RoleID"]).Trim(); }
            set { HttpContext.Current.Session["RoleID"] = value; }
        }
        public static string RoleName
        {
            get { return ((HttpContext.Current.Session["RoleName"] == null) ? string.Empty : (string)HttpContext.Current.Session["RoleName"]).Trim(); }
            set { HttpContext.Current.Session["RoleName"] = value; }
        }
        public static string GACID
        {
            get { return ((HttpContext.Current.Session["GACID"] == null) ? string.Empty : (string)HttpContext.Current.Session["GACID"]).Trim(); }
            set { HttpContext.Current.Session["GACID"] = value; }
        }
        public static string GACName
        {
            get { return ((HttpContext.Current.Session["GACName"] == null) ? string.Empty : (string)HttpContext.Current.Session["GACName"]).Trim(); }
            set { HttpContext.Current.Session["GACName"] = value; }
        }
        public static string RoleTypeId
        {
            get { return ((HttpContext.Current.Session["RoleTypeId"] == null) ? string.Empty : (string)HttpContext.Current.Session["RoleTypeId"]).Trim(); }
            set { HttpContext.Current.Session["RoleTypeId"] = value; }
        }
        public static string RoleTypeName
        {
            get { return ((HttpContext.Current.Session["RoleTypeName"] == null) ? string.Empty : (string)HttpContext.Current.Session["RoleTypeName"]).Trim(); }
            set { HttpContext.Current.Session["RoleTypeName"] = value; }
        }
        //public static string MemberID
        //{
        //    get { return ((HttpContext.Current.Session["MemberID"] == null) ? string.Empty : (string)HttpContext.Current.Session["MemberID"]).Trim(); }
        //    set { HttpContext.Current.Session["MemberID"] = value; }
        //}
        //public static string MemberName
        //{
        //    get { return ((HttpContext.Current.Session["MemberName"] == null) ? string.Empty : (string)HttpContext.Current.Session["MemberName"]).Trim(); }
        //    set { HttpContext.Current.Session["MemberName"] = value; }
        //}
        public static string UserType
        {
            get { return ((HttpContext.Current.Session["UserType"] == null) ? string.Empty : (string)HttpContext.Current.Session["UserType"]).Trim(); }
            set { HttpContext.Current.Session["UserType"] = value; }
        }
        public static string sessionId
        {
            get { return ((HttpContext.Current.Session["sessionId"] == null) ? string.Empty : (string)HttpContext.Current.Session["sessionId"]).Trim(); }
            set { HttpContext.Current.Session["sessionId"] = value; }
        }
        public static string service
        {
            get { return ((HttpContext.Current.Session["service"] == null) ? string.Empty : (string)HttpContext.Current.Session["service"]).Trim(); }
            set { HttpContext.Current.Session["service"] = value; }
        }
        public static string localTokenId
        {
            get { return ((HttpContext.Current.Session["localTokenId"] == null) ? string.Empty : (string)HttpContext.Current.Session["localTokenId"]).Trim(); }
            set { HttpContext.Current.Session["localTokenId"] = value; }
        }
        public static string browserId
        {
            get { return ((HttpContext.Current.Session["browserId"] == null) ? string.Empty : (string)HttpContext.Current.Session["browserId"]).Trim(); }
            set { HttpContext.Current.Session["browserId"] = value; }
        }
       
        public static string departmentName
        {
            get { return ((HttpContext.Current.Session["departmentName"] == null) ? string.Empty : (string)HttpContext.Current.Session["departmentName"]).Trim(); }
            set { HttpContext.Current.Session["departmentName"] = value; }
        }

        public static string authLetter
        {
            get { return ((HttpContext.Current.Session["authLetter"] == null) ? string.Empty : (string)HttpContext.Current.Session["authLetter"]).Trim(); }
            set { HttpContext.Current.Session["authLetter"] = value; }
        }
        public static string appID
        {
            get { return ((HttpContext.Current.Session["appID"] == null) ? string.Empty : (string)HttpContext.Current.Session["appID"]).Trim(); }
            set { HttpContext.Current.Session["appID"] = value; }
        }

        public static string appIsActive
        {
            get { return ((HttpContext.Current.Session["appIsActive"] == null) ? string.Empty : (string)HttpContext.Current.Session["appIsActive"]).Trim(); }
            set { HttpContext.Current.Session["appIsActive"] = value; }
        }
        //Intermediary cred
        public static string IntermediaryId
        {
            get { return ((HttpContext.Current.Session["IntermediaryId"] == null) ? string.Empty : (string)HttpContext.Current.Session["IntermediaryId"]).Trim(); }
            set { HttpContext.Current.Session["IntermediaryId"] = value; }
        }
        public static string IntermediaryTitle
        {
            get { return ((HttpContext.Current.Session["IntermediaryTitle"] == null) ? string.Empty : (string)HttpContext.Current.Session["IntermediaryTitle"]).Trim(); }
            set { HttpContext.Current.Session["IntermediaryTitle"] = value; }
        }
        //public static string URL
        //{
        //    get { return ((HttpContext.Current.Session["URL"] == null) ? string.Empty : (string)HttpContext.Current.Session["URL"]).Trim(); }
        //    set { HttpContext.Current.Session["URL"] = value; }
        //}
        //public static string HelpLink
        //{
        //    get { return ((HttpContext.Current.Session["HelpLink"] == null) ? string.Empty : (string)HttpContext.Current.Session["HelpLink"]).Trim(); }
        //    set { HttpContext.Current.Session["HelpLink"] = value; }
        //}
        //public static string Address
        //{
        //    get { return ((HttpContext.Current.Session["Address"] == null) ? string.Empty : (string)HttpContext.Current.Session["Address"]).Trim(); }
        //    set { HttpContext.Current.Session["Address"] = value; }
        //}

        public static List<IntermediaryForSession> IntermediaryList
        {
            get { return ((HttpContext.Current.Session["IntermediaryList"] == null) ? null : (List<IntermediaryForSession>)HttpContext.Current.Session["IntermediaryList"]); }
            set { HttpContext.Current.Session["IntermediaryList"] = value; }
        }
        //Intermediary cred

        //LangCulture
        public static string LangCulture
        {
            get { return ((HttpContext.Current.Session["LangCulture"] == null) ? "en-IN" : (string)HttpContext.Current.Session["LangCulture"]).Trim(); }
            set { HttpContext.Current.Session["LangCulture"] = value; }
        }

        //LangCulture

    }
}