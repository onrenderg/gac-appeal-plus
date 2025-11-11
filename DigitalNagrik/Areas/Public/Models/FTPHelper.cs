using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class FTPHelper
    {
        public bool FtpDirectoryExist(string dirPath)
        {
            try
            {
                string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
                string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
                string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + dirPath);
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
        public void CreateFTPDir(string Path)
        {
            string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
            string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
            string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + Path);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }
        public Boolean saveFTPfile(byte[] fileBytes, string Path)
        {
            Boolean res = false;
            if (FileExists(Path))
            {
                DeleteFile(Path);
            }
            string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
            string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
            string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + Path);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = fileBytes.Length;
                request.EnableSsl = false;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                res = true;
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
            return res;
        }
        public bool FileExists(string fileName)
        {
            try
            {

                string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
                string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
                string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + fileName);
                request.Method = WebRequestMethods.Ftp.GetFileSize;
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
        public string DeleteFile(string fileName)
        {
            try
            {
                if (FileExists(fileName))
                {

                    string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
                    string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
                    string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + fileName);
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        return response.StatusDescription;
                    }
                }
                else { return ""; }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}