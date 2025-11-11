using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SQLite.SQLite3;

namespace GACAppeal
{
    public partial class App : Application
    {
        public static List<CitizenProfile> listCitizenProfile;
        public static List<CitizenDashboard> listCitizenDashboard;
        public static List<DashboardCount> list_DashboardCount;
        public static List<Groundkeywords> list_Groundkeywords;
        public static List<Models.RefreshMasters> list_RefreshMasters;
        public static string AppName, Btn_OK, Btn_Cancel, NoInternet_, BaseURL_, MobileUpload_BaseURL_, MobileNo;
        public static DateTime AppealDate;
        public App ()
        {
            InitializeComponent();
            AppealDate = DateTime.Now.Date;
            GroundkeywordsDatabase groundkeywordsDatabase = new GroundkeywordsDatabase();
            CitizenProfileDatabase citizenProfileDatabase = new CitizenProfileDatabase();
            citizenProfileDatabase.FillCitizenProfile();
            AppName = "Grievance Appellate Committee";
            Btn_OK = "OK";
            Btn_Cancel = "Cancel";
            NoInternet_ = NoInternet_ = "It Seems that you are not connected to Internet. Please check your internet connection and try again";
            list_Groundkeywords = groundkeywordsDatabase.FillGroundkeywords("").ToList();
#if DEBUG
            BaseURL_ = "http://10.146.2.121/ravi/";
            MobileUpload_BaseURL_ = "http://10.146.2.79/DigitalNagrikAPI/MobileUpload";
#else
            BaseURL_ = "https://gac.gov.in/DigitalNagrikAPI/";
            MobileUpload_BaseURL_ = "https://gac.gov.in/DigitalNagrikFTPAPI/MobileUpload";
#endif
            BaseURL_ = "http://10.146.2.121/ravi/";
            MobileUpload_BaseURL_ = "http://10.146.2.79/DigitalNagrikAPI/MobileUpload";
            if (Preferences.Get("Mobile_No", "") == "")
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else if (Preferences.Get("AadhaarVerified", "N") == "N")
            {
                MainPage = new NavigationPage(new AadhaarPage());
            }
            else
            {
                MainPage = new NavigationPage(new HomePage());
            }
        }
        public static async void update()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    double installedVersionNumber = double.Parse(VersionTracking.CurrentVersion);
                    double latestVersionNumber = installedVersionNumber;

                    var client = new HttpClient();
                    string CurrentPlateform = "A";
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        CurrentPlateform = "I";
                    }

                    var responce = await client.GetAsync($"https://mobileappshp.nic.in/MyDiary/MobileAppVersions.svc/GetAppVersion?&Platform={CurrentPlateform}&packageid={AppInfo.PackageName}");
                    var MyJson = await responce.Content.ReadAsStringAsync();

                    JObject parsed = JObject.Parse(MyJson);
                    var ServiceStatusCode = parsed["message"]["status"].ToString();
                    if (ServiceStatusCode == "200")
                    {
                        if (MyJson.Contains("Mandatory"))
                        {
                            latestVersionNumber = double.Parse(parsed["appVersionDetails"][0]["VersionNumber"].ToString());
                            if (installedVersionNumber < latestVersionNumber)
                            {
                                if (parsed["appVersionDetails"][0]["Mandatory"].ToString() == "Y")
                                {
                                    await App.Current.MainPage.DisplayAlert("New Version", $"There is a new version (v{parsed["appVersionDetails"][0]["VersionNumber"].ToString()}) of this app available.\nWhatsNew: {parsed["appVersionDetails"][0]["WhatsNew"].ToString()}", "Update");
                                    await Launcher.OpenAsync(parsed["appVersionDetails"][0]["Url"].ToString());
                                    return;
                                }
                                else
                                {
                                    var updat = await App.Current.MainPage.DisplayAlert("New Version", $"There is a new version (v{parsed["appVersionDetails"][0]["VersionNumber"].ToString()}) of this app available.\nWhatsNew: {parsed["appVersionDetails"][0]["WhatsNew"].ToString()}\nWould you like to update now?", "Yes", "No");
                                    if (updat)
                                    {
                                        await Launcher.OpenAsync(parsed["appVersionDetails"][0]["Url"].ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                
            }
            
        }
        public static string LableValue(Label label,string lableID,string mandatory = "")
        {
            Models.FormLabelsDatabase formLabelsDatabase = new Models.FormLabelsDatabase();
            try
            {
                label.Text = formLabelsDatabase.FillFormLabels(lableID) + mandatory;
                return label.Text;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string LableValueGeneric(string lableID)
        {
            Models.FormLabelsDatabase formLabelsDatabase = new Models.FormLabelsDatabase();
            try
            {
                var mn = formLabelsDatabase.FillFormLabels(lableID);
                return formLabelsDatabase.FillFormLabels(lableID);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
        public static string Encrypt(string PlainText, string secretKey)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.ECB;
            byte[] keyArr = generateAES256Key(secretKey);
            byte[] KeyArrBytes32Value = new byte[16];
            Array.Copy(keyArr, KeyArrBytes32Value, 16);
            aes.Key = KeyArrBytes32Value;
            ICryptoTransform encrypto = aes.CreateEncryptor();
            byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return Convert.ToBase64String(CipherText);
        }

        public static byte[] generateAES256Key(string seed)
        {
            SHA256 sha256 = SHA256CryptoServiceProvider.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }
        public static async Task<bool> ValidateName(string str, string ReturnMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ProvideInput") + ReturnMessage, App.Btn_OK);
                    return false;
                }
                Regex rgx = new Regex(@"\p{Cs}");
                if (rgx.IsMatch(str))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Emoji") + ReturnMessage, App.Btn_OK);
                    return false;
                }
                if (!str.Replace(" ", String.Empty).All(Char.IsLetter))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_InValidCharacter") + ReturnMessage, App.Btn_OK);
                    return false;
                }
            }
            catch
            {
                await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ProvideInput") + ReturnMessage, App.Btn_OK);
                return false;
            }
            return true;
        }
        public static async Task<bool> ValidateNameWithNullAllow(string str, string ReturnMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return true;
                }
                Regex rgx = new Regex(@"\p{Cs}");
                if (rgx.IsMatch(str))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Emoji") + ReturnMessage, App.Btn_OK);
                    return false;
                }
                if (!str.Replace(" ", String.Empty).All(Char.IsLetter))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_InValidCharacter") + ReturnMessage, App.Btn_OK);
                    return false;
                }
            }
            catch
            {
                await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ProvideInput") + ReturnMessage, App.Btn_OK);
                return false;
            }
            return true;
        }
        public static async Task<bool> ValidateAddress(string str, string ReturnMessage)
        {
            //Validate Address
            string[] SpecialCharactes = { "~", "[", "@", "^", "{", "%", "*", "|", "&", "<", "`", "}", "_", "=", "]", ">", ";", "?", "$", "'", "\"" };
            if (!string.IsNullOrEmpty(str))
            {
                foreach (string x in SpecialCharactes)
                {
                    if (str.Contains(x))
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, $"{App.LableValueGeneric("Mobile_Alert_InValidCharacter")} '{x}' found in {ReturnMessage}!", App.Btn_OK);
                        return false;
                    }
                }
                Regex rgx = new Regex(@"\p{Cs}");
                if (rgx.IsMatch(str))
                {
                    await App.Current.MainPage.DisplayAlert(App.AppName, $"{App.LableValueGeneric("Mobile_Alert_Emoji")} {ReturnMessage}!", App.Btn_OK);
                    return false;
                }
                return true;
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, $"{ReturnMessage} is required", App.Btn_OK);
                return false;
            }
        }
        public static async Task<bool> ValidateEmail(string str, string ReturnMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ProvideInput") + ReturnMessage, App.Btn_OK);
                    return false;
                }
                Regex rgx = new Regex(@"\p{Cs}");
                if (rgx.IsMatch(str.ToLower()))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Emoji") + ReturnMessage, App.Btn_OK);
                    return false;
                }
                Regex rg = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                if (!rg.IsMatch(str.ToLower()))
                {
                    await Current.MainPage.DisplayAlert(App.AppName, "Please provide valid emailID in " + ReturnMessage, App.Btn_OK);
                    return false;
                }
            }
            catch
            {
                await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ProvideInput") + ReturnMessage, App.Btn_OK);
                return false;
            }
            return true;
        }
        public static async Task<bool> ValidatePINCode(string pinCode, string ReturnMessage)
        {

            // Regex to check valid pin code of India.
            Regex rg = new Regex(@"^[1-9]{1}[0-9]{5}$");
            // If the pin code is empty
            // return false
            if (string.IsNullOrEmpty(pinCode))
            {
                await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ProvideInput") + ReturnMessage, App.Btn_OK);
                return false;
            }
            // Return true if the pin code
            // matched the ReGex
            if (rg.IsMatch(pinCode))
            {
                return true;
            }
            else
            {
                await Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_InValidCharacter") + ReturnMessage, App.Btn_OK);
                return false;
            }
        }
    }
}

