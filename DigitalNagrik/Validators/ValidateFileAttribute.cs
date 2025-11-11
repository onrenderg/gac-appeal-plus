using DigitalNagrik.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Validators
{
    public class ValidateFileAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int MaxSizeLimit = (40 * 1024);
        private string _FileTypes { get; set; }
        private string _MaxFileSize { get; set; }
        private string[] _RequiredhexSignatures { get; set; }

        private static readonly ReadOnlyDictionary<string, string[]> _FileHexSignatures = new ReadOnlyDictionary<string, string[]>(
            new Dictionary<string, string[]>()
            {
                 { "PDF", new string[] { "25-50-44-46" } },
                 { "JPG", new string[] { "FF-D8-FF-DB", "FF-D8-FF-E0", "FF-D8-FF-E1", "FF-D8-FF-E2", "FF-D8-FF-E3", "FF-D8-FF-E8" } },
                 { "JPEG", new string[] { "FF-D8-FF-DB", "FF-D8-FF-E0", "FF-D8-FF-E1", "FF-D8-FF-E2", "FF-D8-FF-E3", "FF-D8-FF-E8" } },
                 { "PNG", new string[] { "89-50-4E-47" } },
                 { "GIF", new string[] { "47-49-46-38" } },
                 { "MP4", new string[] { "66-74-79-70", "00-00-00-18" } },
                 //{ "AVI", new string[] { "52-49-46-46", "41-56-49-20" } },
                 //{ "WMV", new string[] { "30-26-B2-75", "A6-D9-00-AA" } },
            }
        );

        private static readonly ReadOnlyDictionary<string, string> _FileExtensionsOnHex = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>()
            {
                 { "25-50-44-46", "PDF" },
                 { "FF-D8-FF-DB", "JPG" }, { "FF-D8-FF-E0", "JPG" }, { "FF-D8-FF-E1", "JPG" }, { "FF-D8-FF-E2", "JPG" }, { "FF-D8-FF-E3", "JPG" }, { "FF-D8-FF-E8", "JPG" },
                 { "89-50-4E-47", "PNG" },
                 { "47-49-46-38", "GIF" },
                 { "66-74-79-70", "MP4" }, { "00-00-00-18", "MP4" },
                 //{ "41-56-49-20", "AVI" }, { "52-49-46-46", "AVI" },
                 //{ "A6-D9-00-AA", "WMV" }, { "30-26-B2-75", "WMV" },
            }
        );

        static string[] FileTypesToHexSignatures(string FileTypes)
        {
            string[] HexSignatures = new string[] { };
            try
            {
                string[] AllFileTypes = FileTypes.Split('|');
                if (AllFileTypes.Length > 0)
                {
                    List<string> HexSignaturesList = new List<string>();
                    for (int i = 0; i < AllFileTypes.Length; i++)
                    {
                        string[] MatchingHexes = _FileHexSignatures[AllFileTypes[i]];
                        HexSignaturesList.AddRange(MatchingHexes);
                    }
                    HexSignatures = HexSignaturesList.ToArray();
                }
                return HexSignatures;
            }
            catch (Exception ex) { }
            return HexSignatures;
        }

        public ValidateFileAttribute(string FileTypes, string MaxFileSize)
        {
            _RequiredhexSignatures = FileTypesToHexSignatures(FileTypes.ToUpper());
            _FileTypes = FileTypes.ToUpper();
            _MaxFileSize = MaxFileSize;
        }

        public override bool IsValid(object value)
        {
            bool FileTypeValid = false;
            bool FileSizeValid = false;
            try
            {
                if (value == null)
                {
                    FileTypeValid = true;
                    FileSizeValid = true;
                }
                else
                {
                    HttpPostedFileBase file = (HttpPostedFileBase)value;
                    if (file.ContentLength > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(_MaxFileSize))
                        {
                            if (_MaxFileSize.ToLower() == "any")
                                FileSizeValid = file.ContentLength <= MaxSizeLimit * 1024 && file.ContentLength > 10;
                            else
                            {
                                int FileSize = int.Parse(_MaxFileSize);
                                FileSizeValid = file.ContentLength <= FileSize * 1024 && file.ContentLength > 10;
                            }
                        }
                        if (FileSizeValid)
                        {
                            if (!string.IsNullOrWhiteSpace(_FileTypes))
                            {
                                if (_RequiredhexSignatures.Length > 0)
                                {
                                    int fileLength = file.ContentLength;
                                    byte[] fileBytes = new byte[4];
                                    file.InputStream.Read(fileBytes, 0, 4);
                                    file.InputStream.Position = 0;
                                    string filehex = BitConverter.ToString(fileBytes);
                                    filehex = filehex.Substring(0, 11);
                                    FileTypeValid = Array.IndexOf(_RequiredhexSignatures, filehex) > -1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return FileTypeValid && FileSizeValid;
        }

        public static bool IsFileTypeValid(HttpPostedFileBase file)
        {
            bool FileTypeValid = false;
            try
            {
                if (file == null)
                {
                    FileTypeValid = true;
                }
                else
                {
                    //  HttpPostedFileBase file = (HttpPostedFileBase)value;
                    if (file.ContentLength > 0)
                    {
                        string FileType = System.IO.Path.GetExtension(file.FileName).Replace(".", "").ToUpper();

                        if (!string.IsNullOrWhiteSpace(FileType))
                        {
                            int fileLength = file.ContentLength;
                            byte[] fileBytes = new byte[fileLength];
                            file.InputStream.Read(fileBytes, 0, fileLength);
                            file.InputStream.Position = 0;
                            string filehex = BitConverter.ToString(fileBytes);
                            if (filehex.Length >= 11)
                            {
                                filehex = filehex.Substring(0, 11);
                                string RequiredhexSignatures = Array.Find(_FileHexSignatures[FileType], element => element == filehex);
                                if (RequiredhexSignatures != null)
                                {
                                    //byte[] Contentfile = new byte[file.ContentLength];
                                    //file.InputStream.Read(Contentfile, 0, file.ContentLength);
                                    string CmsContentFile = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                                    if (CmsContentFile.Contains("alert") || CmsContentFile.Contains("ALERT") || CmsContentFile.Contains("/js") || CmsContentFile.Contains("/JS") || CmsContentFile.Contains("/JavaScript") || CmsContentFile.Contains("/JAVASCRIPT") || CmsContentFile.Contains("/SCRIPT"))
                                    {
                                        FileTypeValid = false;
                                    }
                                    else
                                    {
                                        FileTypeValid = true;
                                    }
                                }

                            }

                        }
                    }

                }
            }

            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return FileTypeValid;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, _FileTypes.ToLower(), _MaxFileSize == "any" ? "any size" : _MaxFileSize + " KB or less");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = string.Format(ErrorMessageString, _FileTypes.ToLower(), _MaxFileSize == "any" ? MaxSizeLimit + " MB or less" : _MaxFileSize + " KB or less"),
                ValidationType = "filevalidation"
            };
            rule.ValidationParameters["filetypes"] = _FileTypes;
            rule.ValidationParameters["maxfilesize"] = _MaxFileSize;
            var URL = new UrlHelper(HttpContext.Current.Request.RequestContext);
            rule.ValidationParameters["urlforfilehex"] = URL.Action("GetFileHexCode", "ValidateFile", new { Area = "" });
            yield return rule;
        }

        public static string GetFileExtensionFromHex(HttpPostedFileBase file)
        {
            string fileExtension = string.Empty;
            if (file != null)
            {
                try
                {
                    if (file.ContentLength > 0)
                    {
                        int fileLength = file.ContentLength;
                        byte[] fileBytes = new byte[fileLength];
                        file.InputStream.Read(fileBytes, 0, fileLength);
                        file.InputStream.Position = 0;
                        string filehex = BitConverter.ToString(fileBytes);
                        filehex = filehex.Substring(0, 11);
                        fileExtension = _FileExtensionsOnHex[filehex].ToLower();
                    }
                }
                catch (Exception ex) { }
            }
            return fileExtension;
        }

        public static bool CheckMimeType(string FileTypes, HttpPostedFileBase file)
        {
            byte[] fileBytes = new byte[4];
            file.InputStream.Read(fileBytes, 0, 4);
            file.InputStream.Position = 0;
            string filehex = BitConverter.ToString(fileBytes);
            filehex = filehex.Substring(0, 11);
            string[] _RequiredhexSignatures = FileTypesToHexSignatures(FileTypes.ToUpper());
            return Array.IndexOf(_RequiredhexSignatures, filehex) > -1;
        }
    }

    public class ValidateFileController : Controller
    {
        [HttpPost]
        public JsonResult GetFileHexCode(HttpPostedFileBase File)
        {
            string filehex = "";
            try
            {
                if (File != null)
                {
                    int fileLength = File.ContentLength;
                    byte[] fileBytes = new byte[fileLength];
                    File.InputStream.Read(fileBytes, 0, fileLength);
                    File.InputStream.Position = 0;
                    filehex = BitConverter.ToString(fileBytes);
                    if (filehex.Length >= 11)
                        filehex = filehex.Substring(0, 11);
                }
            }
            catch (Exception ex) { }
            return Json(filehex, JsonRequestBehavior.AllowGet);
        }
    }
}
