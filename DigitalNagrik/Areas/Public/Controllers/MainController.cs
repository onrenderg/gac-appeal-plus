using DigitalNagrik.Areas.Public.Models;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace DigitalNagrik.Areas.Public.Controllers
{
    public class MainController : Controller
    {
        
        public void checkMessage(string Message, string MessageDesc)
        {
            if (Message != null)
            {
                if (Message == "LoginSuccess")
                {
                    ViewData["SuccessMessage"] = "Logged in successfully";
                }
                else if (Message == "OTPSent")
                {
                    ViewData["SuccessMessage"] = "OTP sent to your mobile successfully";
                }
                else if (Message == "OTPVerified")
                {
                    ViewData["SuccessMessage"] = "OTP Verified successfully";
                }
                else if (Message == "AppealDeleted")
                {
                    ViewData["SuccessMessage"] = "Appeal detail deleted successfully";
                }
                else if (Message == "PinSet")
                {
                    ViewData["SuccessMessage"] = "User Pin saved successfully";
                }
                else if (Message == "ProfileSaved")
                {
                    ViewData["SuccessMessage"] = "Profile detail saved successfully";
                }
                else if (Message == "DraftSuccess")
                {
                    ViewData["SuccessMessage"] = "Draft Appeal saved successfully";
                }
                else if (Message == "IntermediarySuccess")
                {
                    ViewData["SuccessMessage"] = "Appeal detail saved successfully";
                }
                else if (Message == "FinalSubmitSuccess")
                {
                    if (Session["FinalSubmitSuccess"] != null)
                    {
                        ViewData["SuccessMessage"] = HttpContext.Session["FinalSubmitSuccess"].ToString();
                    }
                    else
                    {
                        ViewData["SuccessMessage"] = "Appeal detail submitted successfully";
                    }
                }
                else if (Message == "ErrorFinalSubmit")
                {
                    ViewData["ErrorMessage"] = MessageDesc;
                }
                else if (Message == "FileDeleted")
                {
                    ViewData["ErrorMessage"] = "File removed successfully";
                }
                else if (Message == "AppealDetailSuccess")
                {
                    ViewData["SuccessMessage"] = "Appeal detail saved successfully";
                }
                else if (Message == "ErrorOnSaving")
                {
                    ViewData["ErrorMessage"] = "An error occured while saving detail, Please try again later";
                }
                else if (Message == "ErrorFileDelete")
                {
                    ViewData["ErrorMessage"] = "Error occured while deleting file";
                }
                else if (Message == "SuccessFileDelete")
                {
                    ViewData["SuccessMessage"] = "Document deleted successfully";
                }
                else if (Message == "ErrorFileUpload")
                {
                    ViewData["ErrorMessage"] = "An error occured while uploading document";
                }
                else if (Message == "SuccessFileUpload")
                {
                    ViewData["SuccessMessage"] = "File uploaded successfully";
                }
                else if (Message == "ErrorURLUpload")
                {
                    ViewData["SuccessMessage"] = "Please enter valid URL";
                }
                else if (Message == "URLSavedSuccess")
                {
                    ViewData["SuccessMessage"] = "URL saved successfully";
                }
                else if (Message == "SuccessURLDelete")
                {
                    ViewData["SuccessMessage"] = "URL removed successfully";
                }
                else if (Message == "FileMandatory")
                {
                    ViewData["ErrorMessage"] = "Please upload mandatory files";
                }
                else if (Message == "DocumentSuccess")
                {
                    ViewData["SuccessMessage"] = "Files Uploaded Successfully";
                }


            }
        }
        public string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
        public static string replaceCharacter(string str)
        {
            List<char> charsToRemove = new List<char>() { '@', '_', ',', '(', ')', ' ' , '#' };

            return String.Concat(str.Split(charsToRemove.ToArray()));
        }



    }
}