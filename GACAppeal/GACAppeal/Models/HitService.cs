using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Security.Cryptography;
using System.Web;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Linq; 
//
using static System.Net.WebRequestMethods;

namespace GACAppeal.Models
{
	public class HitService
	{
        //OTP
        public async Task<int> OTP_Post(string _mobile_no)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        MobileNo = AESCryptography.EncryptAES(_mobile_no),
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/OTP?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201)
                    {
                        Preferences.Set("otp_id", parsed["data"]["otp_id"].ToString());
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> OTP_Put(string _MobileNo, string _txtMobileOTP)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        MobileNo = AESCryptography.EncryptAES(_MobileNo),
                        MobileNoOTPID = AESCryptography.EncryptAES(Preferences.Get("otp_id", "0000")),
                        txtMobileOTP = AESCryptography.EncryptAES(_txtMobileOTP)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PutAsync(App.BaseURL_ + "api/OTP?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 200 || (int)response.StatusCode == 201)
                    {
                        CitizenProfileDatabase citizenProfileDatabase = new CitizenProfileDatabase();
                        citizenProfileDatabase.DeleteCitizenProfile();
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new CitizenProfile();
                                    item.UserId = AESCryptography.DecryptAES(node["UserID"].ToString());
                                    item.UserProfilleId =  AESCryptography.DecryptAES(node["UserProfileID"].ToString());
                                    citizenProfileDatabase.AddCitizenProfile(item);
                                }
                            }
                        }
                        citizenProfileDatabase.FillCitizenProfile();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //OTP
        public async Task<int> PIN_Post(string _mobile_no, string _PIN)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        MobileNo = AESCryptography.EncryptAES(_mobile_no),
                        PIN = AESCryptography.EncryptAES(ComputeSha256Hash(_PIN))
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/PIN?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {
                        
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }
        //Groundkeywords
        public async Task<int> Groundkeywords_Get(string SearchString = "")
        {
            GroundkeywordsDatabase groundkeywordsDatabase = new GroundkeywordsDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/Groundkeywords?SearchString={HttpUtility.UrlEncode(AESCryptography.EncryptAES(SearchString))}");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                groundkeywordsDatabase.DeleteGroundkeywords();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new Groundkeywords();
                                    item.Keyword = AESCryptography.DecryptAES(node["Keyword"].ToString());
                                    groundkeywordsDatabase.AddGroundkeywords(item);
                                }
                            }
                        }
                        App.list_Groundkeywords = groundkeywordsDatabase.FillGroundkeywords("").ToList();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //AadhaarValidation
        public async Task<int> AadharNameAndNumberVerification_Post(string _aadhaarNumber,string _AadhaarName)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        password = "Welcome@123",
                        aadhaarNumber = _aadhaarNumber,
                        txnRequestID = Guid.NewGuid().ToString().Trim(),
                        bioAuth = "n",
                        demoAuth = "y",
                        name = _AadhaarName
                    });
                    //end parameters
                    jsonData = App.Encrypt(jsonData, "df0025ae-77d8-4806-aa72-ee7610b00bf5");
                    string jsonData1 = JsonConvert.SerializeObject(new
                    {
                        serviceId = "10003",
                        data = jsonData
                        //otpMedium = _otpMedium
                    });
                    jsonData = jsonData1;
                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync("https://authenticate.epramaan.gov.in/authwebservice/requestauth", content);
                    //API Response Received
                    // if response is 200 then 

                    if ((int)response.StatusCode == 404)
                    {
                        await App.Current.MainPage.DisplayAlert("Title", $"{response}", "OK");




                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {
                        //{"reqTransactionID":"23f5eaab-e419-4f21-8c93-e72d627cdd3d","status":true,"otpTransactionID":"e-Pramaan:cdac/31Jan2023/3963024586266098","errorCode":null}
                        if ((parsed["status"].ToString().ToLower() ?? "").Trim() == "true")
                        {
                            return 200;
                        }
                        else
                        {
#if DEBUG
                            await App.Current.MainPage.DisplayAlert(App.AppName, result, App.Btn_OK);
#else
                            await App.Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_service"), App.Btn_OK);
#endif
                            return 300;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_service"), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> AadharVerification_Post(string _aadhaarNumber)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        aadhaarNumber = _aadhaarNumber,
                        password = "Welcome@123",
                        txnRequestID = Guid.NewGuid().ToString().Trim()
                    });
                    //end parameters
                    jsonData = App.Encrypt(jsonData, "df0025ae-77d8-4806-aa72-ee7610b00bf5");
                    string jsonData1 = JsonConvert.SerializeObject(new
                    {
                        serviceId = "10003",
                        data = jsonData
                        //otpMedium = _otpMedium
                    });
                    jsonData = jsonData1;
                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync("https://department.epramaan.gov.in/authwebservice/requestaadhaarotpauth", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    await AadharVerificationLog_Post("M_requestaadhaarotpauth", result);
                    var parsed = JObject.Parse(result);
                    
                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {
                        //{"reqTransactionID":"23f5eaab-e419-4f21-8c93-e72d627cdd3d","status":true,"otpTransactionID":"e-Pramaan:cdac/31Jan2023/3963024586266098","errorCode":null}
                        if ((parsed["status"].ToString().ToLower() ?? "").Trim() == "true")
                        {
                            Preferences.Set("otpTransactionID", parsed["otpTransactionID"].ToString());
                            Preferences.Set("AadhaarVerified", "M");
                        }
                        else
                        {
#if DEBUG
                            await App.Current.MainPage.DisplayAlert(App.AppName, result, App.Btn_OK);
#else
                            await App.Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_service"), App.Btn_OK);
#endif
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_service"), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> AadharVerification_Put(string _aadhaarNumber,string _otp)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        aadhaarNumber = _aadhaarNumber,
                        password = "Welcome@123",
                        txnRequestID = Guid.NewGuid().ToString().Trim(),
                        otpTxnID = Preferences.Get("otpTransactionID", ""),
                        otp = _otp
                    });
                    //end parameters
                    jsonData = App.Encrypt(jsonData, "df0025ae-77d8-4806-aa72-ee7610b00bf5");
                    string jsonData1 = JsonConvert.SerializeObject(new
                    {
                        serviceId = "10003",
                        data = jsonData
                        //otpMedium = _otpMedium
                    });
                    jsonData = jsonData1;
                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync("https://department.epramaan.gov.in/authwebservice/verifyaadhaarotpauth", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    await AadharVerificationLog_Post("M_verifyaadhaarotpauth", result);
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {
                        //{"reqTransactionID":"23f5eaab-e419-4f21-8c93-e72d627cdd3d","status":true,"otpTransactionID":"e-Pramaan:cdac/31Jan2023/3963024586266098","errorCode":null}
                        if ((parsed["status"].ToString().ToLower() ?? "").Trim() == "true")
                        {
                            Preferences.Set("AadhaarVerified", "Y");

                        }
                        else
                        {
#if DEBUG
                            await App.Current.MainPage.DisplayAlert(App.AppName, result, App.Btn_OK);
#else
                            await App.Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_service"), App.Btn_OK);
#endif
                        }

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_service"), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> AadharVerification_Post(string _UserID, string _FirstName, string _MiddleName, string _LastName)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        UserID = AESCryptography.EncryptAES(_UserID),
                        FirstName = AESCryptography.EncryptAES(_FirstName),
                        MiddleName = AESCryptography.EncryptAES(_MiddleName),
                        LastName = AESCryptography.EncryptAES(_LastName)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/AadharVerification?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> AadharVerificationLog_Post(string _RequestMethod, string _Responsetext)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RequestMethod = AESCryptography.EncryptAES(_RequestMethod),
                        Responsetext = AESCryptography.EncryptAES(_Responsetext)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/AadharVerificationLog?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {

                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //CitizenProfile
        public async Task<int> CitizenProfile_Post(string _UserMobile, string _FirstName,string _MiddleName,string _LastName, string _UserEmail, string _Address, string _Tehsil, string _DistrictId, string _StateId, string _PinCode, string _Occupation)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        UserMobile = AESCryptography.EncryptAES(_UserMobile),
                        FirstName = AESCryptography.EncryptAES(_FirstName),
                        MiddleName = AESCryptography.EncryptAES(_MiddleName),
                        LastName = AESCryptography.EncryptAES(_LastName),
                        UserEmail = AESCryptography.EncryptAES(_UserEmail),
                        Address = AESCryptography.EncryptAES(_Address),
                        Tehsil = AESCryptography.EncryptAES(_Tehsil),
                        DistrictId = AESCryptography.EncryptAES(_DistrictId),
                        StateId = AESCryptography.EncryptAES(_StateId),
                        PinCode = AESCryptography.EncryptAES(_PinCode),
                        Occupation = AESCryptography.EncryptAES(_Occupation),
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/CitizenProfile?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }
        public async Task<int> CitizenProfile_Get(string _MobileNo)
        {
            CitizenProfileDatabase citizenProfileDatabase = new CitizenProfileDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/CitizenProfile?MobileNo={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_MobileNo))}");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                citizenProfileDatabase.DeleteCitizenProfile();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new CitizenProfile();
                                    item.UserId = AESCryptography.DecryptAES(node["UserId"].ToString());
                                    item.UserProfilleId = AESCryptography.DecryptAES(node["UserProfilleId"].ToString());
                                    item.FirstName = AESCryptography.DecryptAES(node["FirstName"].ToString());
                                    item.MiddleName = AESCryptography.DecryptAES(node["MiddleName"].ToString());
                                    item.LastName = AESCryptography.DecryptAES(node["LastName"].ToString());
                                    item.UserMobile = AESCryptography.DecryptAES(node["UserMobile"].ToString());
                                    item.UserEmail = AESCryptography.DecryptAES(node["UserEmail"].ToString());
                                    item.AadhaarVerificationStatus = AESCryptography.DecryptAES(node["AadhaarVerificationStatus"].ToString());
                                    citizenProfileDatabase.AddCitizenProfile(item);
                                }
                            }
                        }
                        citizenProfileDatabase.FillCitizenProfile();
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //CitizenDashboard
        public async Task<int> CitizenDashboard_Get(string _MobileNo)
        {
            CitizenDashboardDatabase citizenDashboardDatabase = new CitizenDashboardDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/CitizenDashboard?UserMobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_MobileNo))}");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                citizenDashboardDatabase.DeleteCitizenDashboard();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new CitizenDashboard();
                                    item.RegistrationYear = AESCryptography.DecryptAES(node["RegistrationYear"].ToString());
                                    item.GrievanceId = AESCryptography.DecryptAES(node["GrievanceId"].ToString());
                                    item.FirstName = AESCryptography.DecryptAES(node["FirstName"].ToString());
                                    item.MiddleName = AESCryptography.DecryptAES(node["MiddleName"].ToString());
                                    item.LastName = AESCryptography.DecryptAES(node["LastName"].ToString());
                                    item.UserMobile = AESCryptography.DecryptAES(node["UserMobile"].ToString());
                                    item.UserEmail = AESCryptography.DecryptAES(node["UserEmail"].ToString());
                                    item.StatusId = AESCryptography.DecryptAES(node["StatusId"].ToString());
                                    item.StatusTitle = AESCryptography.DecryptAES(node["StatusTitle"].ToString());
                                    item.GrievanceDesc = AESCryptography.DecryptAES(node["GrievanceDesc"].ToString());
                                    item.ReasonForAppeal = AESCryptography.DecryptAES(node["ReasonForAppeal"].ToString());
                                    item.IntermediaryURL = AESCryptography.DecryptAES(node["IntermediaryURL"].ToString());
                                    item.UserId = AESCryptography.DecryptAES(node["UserId"].ToString());
                                    item.UserProfileId = AESCryptography.DecryptAES(node["UserProfileId"].ToString());
                                    item.CaseHistoryFilePath = AESCryptography.DecryptAES(node["CaseHistoryFilePath"].ToString());
                                    item.CaseHistoryFileType = AESCryptography.DecryptAES(node["CaseHistoryFileType"].ToString());
                                    item.IntermediaryId = AESCryptography.DecryptAES(node["IntermediaryId"].ToString());
                                    item.IntermediaryTitle = AESCryptography.DecryptAES(node["IntermediaryTitle"].ToString());
                                    item.IntermediaryGROName = AESCryptography.DecryptAES(node["IntermediaryGROName"].ToString());
                                    item.EmailOf = AESCryptography.DecryptAES(node["EmailOf"].ToString());
                                    item.IntermediaryGROEmail = AESCryptography.DecryptAES(node["IntermediaryGROEmail"].ToString());
                                    item.IntermediaryAddress = AESCryptography.DecryptAES(node["IntermediaryAddress"].ToString());
                                    item.IntermediaryDate = AESCryptography.DecryptAES(node["IntermediaryDate"].ToString());
                                    item.GrievnaceStatus = AESCryptography.DecryptAES(node["GrievnaceStatus"].ToString());
                                    item.ReceiptDate = AESCryptography.DecryptAES(node["ReceiptDate"].ToString());
                                    item.LastUpdatedOn = AESCryptography.DecryptAES(node["LastUpdatedOn"].ToString());
                                    item.Justification = AESCryptography.DecryptAES(node["Justification"].ToString());
                                    item.ReliefSoughtID = AESCryptography.DecryptAES(node["ReliefSoughtID"].ToString());
                                    item.ReliefTitle = AESCryptography.DecryptAES(node["ReliefTitle"].ToString());
                                    item.RelieftSoughtSpecification = AESCryptography.DecryptAES(node["RelieftSoughtSpecification"].ToString());
                                    item.GroundAppealID = AESCryptography.DecryptAES(node["GroundAppealID"].ToString());
                                    item.GroundTitle = AESCryptography.DecryptAES(node["GroundTitle"].ToString());
                                    item.ReceiptDateTime = AESCryptography.DecryptAES(node["ReceiptDateTime"].ToString());
                                    item.LastResponseTime = AESCryptography.DecryptAES(node["LastResponseTime"].ToString());
                                    item.GroundAppealLawText = AESCryptography.DecryptAES(node["GroundAppealLawText"].ToString());
                                    item.EntryFieldLabel = AESCryptography.DecryptAES(node["EntryFieldLabel"].ToString());
                                    item.SpecificationLabel = AESCryptography.DecryptAES(node["SpecificationLabel"].ToString());

                                    item.Keyword = AESCryptography.DecryptAES(node["Keyword"].ToString());
                                    item.BriefofComplaint = AESCryptography.DecryptAES(node["BriefofComplaint"].ToString());
                                    item.dateofComplaint = AESCryptography.DecryptAES(node["dateofComplaint"].ToString());
                                    item.dateofDecision = AESCryptography.DecryptAES(node["dateofDecision"].ToString());
                                    item.ComplianceURL = AESCryptography.DecryptAES(node["ComplianceURL"].ToString());
                                    item.DecisionFilePath = AESCryptography.DecryptAES(node["DecisionFilePath"].ToString());

                                    item.AppealID = item.GrievanceId+"/"+ item.RegistrationYear;
                                    citizenDashboardDatabase.AddCitizenDashboard(item);
                                }
                            }
                        }
                        citizenDashboardDatabase.FillCitizenDashboard();
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //DashboardCount
        public async Task<int> DashboardCount_Get()
        {
            DashboardCountDatabase dashboardCountDatabase = new DashboardCountDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/DashboardCount?");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                dashboardCountDatabase = new DashboardCountDatabase();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new DashboardCount();
                                    item.Disposed = AESCryptography.DecryptAES(node["Disposed"].ToString());
                                    item.Submitted = AESCryptography.DecryptAES(node["Submitted"].ToString());
                                    dashboardCountDatabase.AddDashboardCount(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    dashboardCountDatabase.FillDashboardCount();
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }


        //Appeal
        public async Task<int> Appeal_Post(string _UserMobile = "", string _UserID = "", string _UserProfileID = "", string _FirstName = "", string _MiddleName = "", string _LastName = "", string _UserEmail = "")
        {
            Preferences.Set("GrievanceID", "");
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        UserMobile = AESCryptography.EncryptAES(_UserMobile),
                        UserID = AESCryptography.EncryptAES(_UserID),
                        UserProfileID = AESCryptography.EncryptAES(_UserProfileID),
                        FirstName = AESCryptography.EncryptAES(_FirstName),
                        MiddleName = AESCryptography.EncryptAES(_MiddleName),
                        LastName = AESCryptography.EncryptAES(_LastName),
                        UserEmail = AESCryptography.EncryptAES(_UserEmail)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/Appeal?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {
                        Preferences.Set("GrievanceID", parsed["data"][0]["GrievanceID"].ToString());
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> Appeal_Put(string _RegistrationYear, string _GrievanceID, string _UserEmail, string _dateofComplaint, string _dateofDecision, string _IntermediaryId, string _IntermediaryTitle,
            string _URL, string _IntermediaryGROEmail, string _IntermediaryAddress, string _BriefofComplaint, string _Keyword, string _Justification, string _ReliefSpecification)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RegistrationYear = AESCryptography.EncryptAES(_RegistrationYear),
                        GrievanceID = AESCryptography.EncryptAES(_GrievanceID),

                        UserEmail = AESCryptography.EncryptAES(_UserEmail),
                        dateofComplaint = AESCryptography.EncryptAES(_dateofComplaint),
                        dateofDecision = AESCryptography.EncryptAES(_dateofDecision),

                        IntermediaryId = AESCryptography.EncryptAES(_IntermediaryId),
                        IntermediaryTitle = AESCryptography.EncryptAES(_IntermediaryTitle),
                        URL = AESCryptography.EncryptAES(_URL),
                        IntermediaryGROEmail = AESCryptography.EncryptAES(_IntermediaryGROEmail),
                        IntermediaryAddress = AESCryptography.EncryptAES(_IntermediaryAddress),

                        BriefofComplaint = AESCryptography.EncryptAES(_BriefofComplaint),
                        Keyword = AESCryptography.EncryptAES(_Keyword),
                        Justification = AESCryptography.EncryptAES(_Justification),
                        ReliefSpecification = AESCryptography.EncryptAES(_ReliefSpecification)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PutAsync(App.BaseURL_ + "api/Appeal?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode != 200)
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> FinalAppeal(string _RegistrationYear, string _GrievanceID, string _UserMobile, string _IPAddress)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RegistrationYear = AESCryptography.EncryptAES(_RegistrationYear),
                        GrievanceID = AESCryptography.EncryptAES(_GrievanceID),
                        UserMobile = AESCryptography.EncryptAES(_UserMobile),
                        IPAddress = AESCryptography.EncryptAES(_IPAddress)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/FinalAppeal?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {
                        //Preferences.Set("GrievanceID", parsed["Message"].ToString());
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //AppealEvidence
        public async Task<int> AppealDocument_Post(string _RegistrationYear, string _GrievanceID, string _File_ID, string _FilePath, string _DocumentType)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RegistrationYear = AESCryptography.EncryptAES(_RegistrationYear),
                        GrievanceID = AESCryptography.EncryptAES(_GrievanceID),
                        File_ID = AESCryptography.EncryptAES(_File_ID),
                        FilePath = AESCryptography.EncryptAES(_FilePath),
                        DocumentType = AESCryptography.EncryptAES(_DocumentType),
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/AppealDocument?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {

                    } 
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> AppealDocument_Get(string _UserID)
        {
            AppealDocumentDatabase appealDocumentDatabase = new AppealDocumentDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/AppealDocument?UserID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_UserID))}");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode != 200)
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    else
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                appealDocumentDatabase.DeleteAppealDocument();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new AppealDocument();
                                    item.RegistrationYear = AESCryptography.DecryptAES(node["RegistrationYear"].ToString());
                                    item.GrievanceId = AESCryptography.DecryptAES(node["GrievanceId"].ToString());
                                    item.FileId = AESCryptography.DecryptAES(node["FileId"].ToString());
                                    item.EvidenceTitle = AESCryptography.DecryptAES(node["EvidenceTitle"].ToString());
                                    //item.DocumentTitle = AESCryptography.DecryptAES(node["DocumentTitle"].ToString());
                                    item.FileTypeID = AESCryptography.DecryptAES(node["FileTypeID"].ToString());
                                    item.FilePath = AESCryptography.DecryptAES(node["FilePath"].ToString());
                                    item.FileType = AESCryptography.DecryptAES(node["FileType"].ToString());
                                    item.DocumentType = AESCryptography.DecryptAES(node["DocumentType"].ToString());
                                    
                                    if ((item.DocumentType ?? "") != "IURL")
                                    {
                                        var _FileName = item.FilePath.Split('/').Last();
                                        var _FileExtension = _FileName.Split('.').Last();
                                        item.FileName = _FileName;
                                        item.FileExtension = _FileExtension;
                                    }
                                    
                                    appealDocumentDatabase.AddAppealDocument(item);
                                }
                            }
                        }
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> DeleteAppealDocument_Post(string _RegistrationYear, string _GrievanceID, string _File_ID)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RegistrationYear = AESCryptography.EncryptAES(_RegistrationYear),
                        GrievanceID = AESCryptography.EncryptAES(_GrievanceID),
                        File_ID = AESCryptography.EncryptAES(_File_ID)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/DeleteAppealDocument?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> DeleteAppealCitizen_Post(string _RegistrationYear, string _GrievanceID, string _UserMobile)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var m = Preferences.Get("BasicAuth", "xx:xx");
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //request parameters as Json
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RegistrationYear = AESCryptography.EncryptAES(_RegistrationYear),
                        GrievanceID = AESCryptography.EncryptAES(_GrievanceID),
                        UserMobile = AESCryptography.EncryptAES(_UserMobile)
                    });
                    //end parameters

                    //Send Json with request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //End Json request

                    //awaited for API Response
                    HttpResponseMessage response = await client.PostAsync(App.BaseURL_ + "api/DeleteAppealCitizen?", content);
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);

                    if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
                    {

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif

                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }
        //SET PIN
        public async Task<int> PIN_Get(string _UserMobile,string _PIN)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client
                    string MySalt = DateTime.Now.ToString("yyMMddHHmmsss");
                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/PIN?UserMobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_UserMobile))}&PIN={HttpUtility.UrlEncode(AESCryptography.EncryptAES(ComputeSha256Hash(MySalt + ComputeSha256Hash(_PIN))))}&Salt={HttpUtility.UrlEncode(AESCryptography.EncryptAES(MySalt))}");
                    //API Response Received
                    Console.WriteLine($"api/PIN?UserMobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_UserMobile))}&PIN={HttpUtility.UrlEncode(AESCryptography.EncryptAES(ComputeSha256Hash(MySalt + ComputeSha256Hash(_PIN))))}&Salt={HttpUtility.UrlEncode(AESCryptography.EncryptAES(MySalt))}");
                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> IntermediaryMaster_Get(string _IntermediaryType = "0")
        {
            IntermediaryMasterDatabase intermediaryMasterDatabase = new IntermediaryMasterDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/IntermediaryMaster?IntermediaryType={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_IntermediaryType))}");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                      var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                intermediaryMasterDatabase.DeleteIntermediaryMaster(_IntermediaryType);
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new IntermediaryMaster();
                                    item.IntermediaryId = AESCryptography.DecryptAES(node["IntermediaryId"].ToString());
                                    item.IntermediaryTitle = AESCryptography.DecryptAES(node["IntermediaryTitle"].ToString());
                                    item.URL = AESCryptography.DecryptAES(node["URL"].ToString());
                                    item.GOName = AESCryptography.DecryptAES(node["GOName"].ToString());
                                    item.Address = AESCryptography.DecryptAES(node["Address"].ToString());
                                    item.GOEmail = AESCryptography.DecryptAES(node["GOEmail"].ToString());
                                    item.HelpLink = AESCryptography.DecryptAES(node["HelpLink"].ToString());
                                    intermediaryMasterDatabase.AddIntermediaryMaster(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }


        public async Task<int> FormLabels_Get(string _IntermediaryType = "0")
        {
            FormLabelsDatabase formLabelsDatabase = new FormLabelsDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/FormLabels?");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                formLabelsDatabase.DeleteFormLabels();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new FormLabels();
                                    item.ModuleID = AESCryptography.DecryptAES(node["ModuleID"].ToString());
                                    item.FormID = AESCryptography.DecryptAES(node["FormID"].ToString());
                                    item.LabelID = AESCryptography.DecryptAES(node["LabelID"].ToString());
                                    item.LabelText = AESCryptography.DecryptAES(node["LabelText"].ToString());
                                    item.LanguageCode = AESCryptography.DecryptAES(node["LanguageCode"].ToString());
                                    formLabelsDatabase.AddFormLabels(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }


        public async Task<int> LanguageMaster_Get()
        {
            LanguageMasterDatabase languageMasterDatabase = new LanguageMasterDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/LanguageMaster");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                languageMasterDatabase.DeleteLanguageMaster();
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new LanguageMaster();
                                    item.LanguageCode = AESCryptography.DecryptAES(node["LanguageCode"].ToString());
                                    item.languageDescription = AESCryptography.DecryptAES(node["languageDescription"].ToString());
                                    item.languageDescriptionlocal = AESCryptography.DecryptAES(node["languageDescriptionlocal"].ToString());
                                    languageMasterDatabase.AddLanguageMaster(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        public async Task<int> RefreshMasters_Get()
        {
            RefreshMastersDatabase refreshMastersDatabase = new RefreshMastersDatabase();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    //Added Basic Auth to client
                    var byteArray = Encoding.ASCII.GetBytes(Preferences.Get("BasicAuth", "xx:xx"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //End basic Auth to client

                    //awaited for API Response
                    HttpResponseMessage response = await client.GetAsync(App.BaseURL_ + $"api/RefreshMasters");
                    //API Response Received

                    var result = await response.Content.ReadAsStringAsync();
                    var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "data")
                            {
                                var nodes = pair.Value;
                                foreach (var node in nodes)
                                {
                                    var item = new RefreshMasters();
                                    item.master_name = AESCryptography.DecryptAES(node["master_name"].ToString());
                                    item.last_updated = AESCryptography.DecryptAES(node["last_updated"].ToString());
                                    item.mandatory = AESCryptography.DecryptAES(node["mandatory"].ToString());
                                    refreshMastersDatabase.AddRefreshMasters(item);
                                }   
                            }
                        }
                        refreshMastersDatabase.FillRefreshMasters();
                        if (refreshMastersDatabase.UpdateMaster("FormLabels"))
                        {
                            if (await FormLabels_Get() == 200)
                            {
                                refreshMastersDatabase.UpdateRefreshMasters("FormLabels");
                                refreshMastersDatabase.FillRefreshMasters();
                            }
                        }
                        if (refreshMastersDatabase.UpdateMaster("Groundkeywords"))
                        {
                            if (await Groundkeywords_Get() == 200)
                            {
                                refreshMastersDatabase.UpdateRefreshMasters("Groundkeywords");
                                refreshMastersDatabase.FillRefreshMasters();
                            }
                        }
                        if (refreshMastersDatabase.UpdateMaster("IntermediaryMaster"))
                        {
                            if (await IntermediaryMaster_Get() == 200)
                            {
                                refreshMastersDatabase.UpdateRefreshMasters("IntermediaryMaster");
                                refreshMastersDatabase.FillRefreshMasters();
                            }
                        }
                        if (refreshMastersDatabase.UpdateMaster("LanguageMaster"))
                        {
                            if (await LanguageMaster_Get() == 200)
                            {
                                refreshMastersDatabase.UpdateRefreshMasters("LanguageMaster");
                                refreshMastersDatabase.FillRefreshMasters();
                            }
                        }
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(App.AppName, parsed["Message"].ToString(), App.Btn_OK);
                    }
                    return (int)response.StatusCode;
                }
                catch (Exception ey)
                {
#if DEBUG
                    await App.Current.MainPage.DisplayAlert("Exception", ey.Message, App.Btn_OK);
#else
                    await App.Current.MainPage.DisplayAlert("Exception", "Something went wrong. Please try again!", "OK");
#endif
                    return 500;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_OK);
                return 101;
            }
        }

        //SHA256
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
    
}

