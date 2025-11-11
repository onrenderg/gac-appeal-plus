using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Web;
using System.Text;
using System.Net.Mail;
using NICServiceAdaptor;
using System.Globalization;
using DigitalNagrik.Models;

public sealed class MyExceptionHandler
{
    private MyExceptionHandler()
    {
    }
    public static void LogError(Exception oEx)
    {
        bool blLogCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLog"]);
        if (blLogCheck)
        {
            HandleException(oEx);
        }
    }
    public static void HandleException(Exception ex)
    {
        HttpContext ctxObject = HttpContext.Current;
        string strReqURL = (ctxObject.Request.Url != null) ? ctxObject.Request.Url.ToString() : String.Empty;
        string strReqQS = (ctxObject.Request.QueryString != null) ? ctxObject.Request.QueryString.ToString() : String.Empty;
        string strServerName = String.Empty;
        if (ctxObject.Server.MachineName != null)
        {
            strServerName = ctxObject.Server.MachineName;
        }
        string strUserAgent = (ctxObject.Request.UserAgent != null) ? ctxObject.Request.UserAgent : String.Empty;
        string strUserIP = BALCommon.GetIPAddress();
        string strUserAuthen = (ctxObject.User.Identity.IsAuthenticated.ToString() != null) ? ctxObject.User.Identity.IsAuthenticated.ToString() : String.Empty;
        string strUserName = (ctxObject.User.Identity.Name != null) ? ctxObject.User.Identity.Name : String.Empty;
        string strMessage = string.Empty;
        string strSource = string.Empty;
        string strTargetSite = string.Empty;
        string strStackTrace = string.Empty;
        while (ex != null)
        {
            strMessage = ex.Message;
            strSource = ex.Source;
            strTargetSite = ex.TargetSite.ToString();
            strStackTrace = ex.StackTrace;
            ex = ex.InnerException;
        }
        try
        {
            var methodparameter = new List<KeyValuePair<string, string>>();
            methodparameter.Add(new KeyValuePair<string, string>("@Source", strSource));
            methodparameter.Add(new KeyValuePair<string, string>("@LogDateTime", DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("en-US"))));
            methodparameter.Add(new KeyValuePair<string, string>("@Message", strMessage));
            methodparameter.Add(new KeyValuePair<string, string>("@QueryString", strReqQS));
            methodparameter.Add(new KeyValuePair<string, string>("@TargetSite", strTargetSite));
            methodparameter.Add(new KeyValuePair<string, string>("@StackTrace", strStackTrace));
            methodparameter.Add(new KeyValuePair<string, string>("@ServerName", strServerName));
            methodparameter.Add(new KeyValuePair<string, string>("@RequestURL", strReqURL));
            methodparameter.Add(new KeyValuePair<string, string>("@UserAgent", strUserAgent));
            methodparameter.Add(new KeyValuePair<string, string>("@UserIP", strUserIP));
            methodparameter.Add(new KeyValuePair<string, string>("@UserAuthentication", strUserAuthen));
            methodparameter.Add(new KeyValuePair<string, string>("@UserName", strUserName));

            string result = ServiceAdaptor.GetStringFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "usp_LogExceptionToDB", methodparameter);
        }
        catch (Exception exc)
        {
            //EventLog.WriteEntry(exc.Source, "Database Error From Exception Log!", EventLogEntryType.Error, 65535);
        }
        finally
        {

        }

    }
}