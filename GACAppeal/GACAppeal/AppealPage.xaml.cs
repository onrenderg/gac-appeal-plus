using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GACAppeal
{
    public partial class AppealPage : ContentPage
    {
        CitizenDashboard _citizenDashboard;
        List<AppealDocument> List_AppealDocument;
        List<IntermediaryMaster> List_IntermediaryMaster;
        //List<AppealGroundMaster> List_AppealGroundMaster;
        //List<AppealReliefMaster> List_AppealReliefMaster;
        string FileType = "0";
        public AppealPage(CitizenDashboard citizenDashboard)
        {
            InitializeComponent();
            DatePicker_User_DateOfComplaint.MaximumDate = DateTime.Now;
            DatePicker_User_DateOfDecision.MaximumDate = DateTime.Now;

            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            _citizenDashboard = citizenDashboard;
            Lbl_Title.Text = $"Appeal Number: {citizenDashboard.AppealID ?? ""}";
            Device.BeginInvokeOnMainThread(async () =>
            {
                IntermediaryMasterDatabase intermediaryMasterDatabase = new IntermediaryMasterDatabase();
                List_IntermediaryMaster = intermediaryMasterDatabase.FillIntermediaryType().ToList();
                
                if (!List_IntermediaryMaster.Any())
                {
                    Loading_activity.IsVisible = true;
                    var service = new Models.HitService();
                    await service.IntermediaryMaster_Get();
                    Loading_activity.IsVisible = false;
                    List_IntermediaryMaster = intermediaryMasterDatabase.FillIntermediaryType().ToList();
                }
                Loading_activity.IsVisible = false;
                
                Loading_activity.IsVisible = false;

                FillAppeal();
                FillLable();
            });
            
        }
        protected override void OnAppearing()
        {
            FillLable();
        }
        void FillLable()
        {
            App.AppName = App.LableValueGeneric("GrievanceAppellateCommittee");
            App.Btn_OK = App.LableValueGeneric("Mobile_OK");
            App.Btn_Cancel = App.LableValueGeneric("Cancel");
            App.NoInternet_ = App.LableValueGeneric("Mobile_NoInternet");

            App.LableValue(Lbl_Nav_title, "GrievanceAppellateCommittee");
            App.LableValue(Lbl_PleaseWait, "Mobile_PleaseWait");
            Lbl_Title.Text = $"{App.LableValueGeneric("AppealNumber")}: {_citizenDashboard.AppealID ?? ""}";
            Entry_IntermediaryName.Placeholder = App.LableValueGeneric("SelectRespondentIntermediary");
            App.LableValue(Lbl_Mandatory, "Mobile_Mandatory");
            App.LableValue(Lbl_AppellantDetails, "AppellantDetails");
            App.LableValue(Lbl_FirstName, "FirstName"," *");
            App.LableValue(Lbl_MiddleName, "MiddleName");
            App.LableValue(Lbl_LastName, "LastName");
            App.LableValue(Lbl_MobileNo, "Mobile");
            App.LableValue(Lbl_Email, "Email"," *");
            App.LableValue(Lbl_EmailHint, "EmailHelpMessage");
            App.LableValue(Lbl_Respondent_Intermediary_Details, "RespondentIntermediaryDetails");
            App.LableValue(Lbl_DateOfComplaint, "DateofComplaint");
            App.LableValue(Lbl_DateOfComplaint_Hint, "DateofComplaintHelpMsg");
            App.LableValue(Lbl_DateOfDecision, "DateofDecision");
            App.LableValue(Lbl_DateOfDecision_Hint, "DateofDecisionHelpMsg");
            App.LableValue(Lbl_IntermediaryName, "RespondentIntermediaryName", " *");
            App.LableValue(Lbl_URLIntermediary, "URLRespondentIntermediary", " *");
            App.LableValue(Lbl_EmailIntermediary, "Email", " *");
            App.LableValue(Lbl_ContactDetails_GO_Intermediary, "RespondentIntermediaryContactDetails");
            App.LableValue(Lbl_AppealDetails, "AppealDetails");
            App.LableValue(Lbl_Keyword, "SubjectKeyword", " *");
            App.LableValue(Lbl_Keyword_hint, "SubjectKeywordHelpMsg", " *");
            App.LableValue(Lbl_Compaint, "BriefofComplaint", " *");
            App.LableValue(Lbl_Justification, "Justificationforappeal", " *");
            App.LableValue(Lbl_Relief_Sough, "ReliefSoughtDetails", " *");
            App.LableValue(Lbl_AppleaComplaint, "AppealComplaintRelatedDocuments");
            App.LableValue(Lbl_CopuOfComplaint, "CCIntermediary", " *"); 
            App.LableValue(Lbl_CopuOfComplaint_hint1, "CCIntermediaryHelpMsg", " *"); 
            App.LableValue(Lbl_AnyOtherInformation, "Anyotherrelevantinformation");

            Btn_ChooseCompyOfComplaint.Text = App.LableValueGeneric("UploadDocumentbutton");
            Btn_ChooseCompyOfDecision.Text = App.LableValueGeneric("UploadDocumentbutton");
            Btn_Draft.Text = App.LableValueGeneric("SaveDraft");
            Btn_Save.Text = App.LableValueGeneric("Proceed");
            Btn_CloseBrowser.Text = App.LableValueGeneric("Cancel");
        }
        void Language_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new LanguagePage());
        }
        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new HomePage());
        }
        void User_DateOfComplaint_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            User_DateOfComplaint.Unfocus();
            //User_DateOfComplaint.IsVisible = false;
            //User_DateOfComplaint.IsEnabled = false;
            Device.BeginInvokeOnMainThread(() => {
                DatePicker_User_DateOfComplaint.IsVisible = true;
                DatePicker_User_DateOfComplaint.Focus();
            });
            return;
        }

        void User_DateOfDecision_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            User_DateOfDecision.Unfocus();
            //User_DateOfDecision.IsVisible = false;
            //User_DateOfDecision.IsEnabled = false;
            Device.BeginInvokeOnMainThread(() => {
                DatePicker_User_DateOfDecision.IsVisible = true;

                DatePicker_User_DateOfDecision.Focus();
            });
            return;
        }
        void Entry_IntermediaryName_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Entry_IntermediaryName.Text))
                {
                    Intermediary_Hint.IsVisible = false;
                }
                else
                {
                    if (Entry_IntermediaryName.Text.Length >= 4)
                    {
                        IntermediaryMasterDatabase intermediaryMasterDatabase = new IntermediaryMasterDatabase();
                        var ListToBeFill = intermediaryMasterDatabase.FillIntermediaryByTitle(Entry_IntermediaryName.Text);
                        if (ListToBeFill.Count() != 0)
                        {
                            Intermediary_Hint.ItemsSource = ListToBeFill;
                            Intermediary_Hint.IsVisible = true;


                            Entry_EmailIntermediary.IsEnabled = false;
                            Entry_URLIntermediary.IsEnabled = false;
                            Editor_ContactDetails_GO_Intermediary.IsEnabled = false;
                        }
                        else
                        {
                            Intermediary_Hint.IsVisible = false;
                            Entry_EmailIntermediary.IsEnabled = true;
                            Entry_URLIntermediary.IsEnabled = true;
                            Editor_ContactDetails_GO_Intermediary.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Intermediary_Hint.IsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Intermediary_Hint.IsVisible = false;
            }
        }
        void Entry_IntermediaryName_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Entry_URLIntermediary.Text))
            {
                IntermediaryMasterDatabase intermediaryMasterDatabase = new IntermediaryMasterDatabase();
                var ListToBeFill = intermediaryMasterDatabase.FillIntermediaryByTitle(Entry_IntermediaryName.Text);
                if (ListToBeFill.Count() != 0)
                {
                    var MyElement = ListToBeFill.First();
                    Entry_IntermediaryName.Text = MyElement.IntermediaryTitle;
                    //Intermediary_Hint.IsVisible = false;
                    Entry_EmailIntermediary.Text = MyElement.GOEmail;
                    Entry_URLIntermediary.Text = MyElement.URL;
                    Editor_ContactDetails_GO_Intermediary.Text = MyElement.Address;

                    if (string.IsNullOrEmpty(Entry_EmailIntermediary.Text))
                    {
                        Entry_EmailIntermediary.IsEnabled = true;
                    }
                    else
                    {
                        Entry_EmailIntermediary.IsEnabled = false;
                    }

                    if (string.IsNullOrEmpty(Entry_URLIntermediary.Text))
                    {
                        Entry_URLIntermediary.IsEnabled = true;
                    }
                    else
                    {
                        Entry_URLIntermediary.IsEnabled = false;
                    }

                    if (string.IsNullOrEmpty(Editor_ContactDetails_GO_Intermediary.Text))
                    {
                        Editor_ContactDetails_GO_Intermediary.IsEnabled = true;
                    }
                    else
                    {
                        Editor_ContactDetails_GO_Intermediary.IsEnabled = false;
                    }
                }
                else
                {
                    Entry_EmailIntermediary.IsEnabled = true;
                    Entry_URLIntermediary.IsEnabled = true;
                    Editor_ContactDetails_GO_Intermediary.IsEnabled = true;
                }
                
            }
            Intermediary_Hint.IsVisible = false;
        }
        void Intermediary_Hint_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var MyElement = e.Item as IntermediaryMaster;
            Entry_IntermediaryName.Text = MyElement.IntermediaryTitle;
            //Intermediary_Hint.IsVisible = false;
            Entry_EmailIntermediary.Text = MyElement.GOEmail;
            Entry_URLIntermediary.Text = MyElement.URL;
            Editor_ContactDetails_GO_Intermediary.Text = MyElement.Address;
            Entry_IntermediaryName.Unfocus();
        }
        
        void Btn_ChooseCompyOfComplaint_Clicked(System.Object sender, System.EventArgs e)
        {
            FileType = "CC";
            App.LableValue(Lbl_Title_Browser, "Mobile_Alert_UploadCoC", "");
            var msds = $"{App.MobileUpload_BaseURL_}?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}&DocumentType={HttpUtility.UrlEncode(AESCryptography.EncryptAES("CC"))}&Mobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.UserMobile))}";
            Upload_borwser.Source = $"{App.MobileUpload_BaseURL_}?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}&DocumentType={HttpUtility.UrlEncode(AESCryptography.EncryptAES("CC"))}&Mobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.UserMobile))}&languagecode={Preferences.Get("LanguageCode", "en-IN")}";
            Upload_activity.IsVisible = true;
        }
        void Btn_ChooseCompyOfDecision_Clicked(System.Object sender, System.EventArgs e)
        {
            FileType = "CD";
            //Lbl_Title_Browser.Text = "Upload Decision Copy";//DecisionCopy
            App.LableValue(Lbl_Title_Browser, "DecisionCopy", "");
            Upload_borwser.Source = $"{App.MobileUpload_BaseURL_}?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}&DocumentType={HttpUtility.UrlEncode(AESCryptography.EncryptAES("CD"))}&Mobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.UserMobile))}&languagecode={Preferences.Get("LanguageCode", "en-IN")}";
            Upload_activity.IsVisible = true;
        }

        async void Btn_Draft_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = new Models.HitService();
            string IntermediaryID = "0";
            try
            {
                IntermediaryID = List_IntermediaryMaster.FindAll(x => x.IntermediaryTitle.ToLower() == Entry_IntermediaryName.Text.ToLower()).First().IntermediaryId;
            }
            catch (Exception)
            {
                IntermediaryID = "0";
            }

            Loading_activity.IsVisible = true;
            string DateOfComplaint, DateOfDecision;
            if (User_DateOfComplaint.IsVisible)
            {
                DateOfComplaint = "";
            }
            else
            {
                DateOfComplaint = DatePicker_User_DateOfComplaint.Date.ToString("yyyy-MM-dd");
            }
            if (User_DateOfDecision.IsVisible)
            {
                DateOfDecision = "";
            }
            else
            {
                DateOfDecision = DatePicker_User_DateOfDecision.Date.ToString("yyyy-MM-dd");
            }
            
            var MyResponce = await service.Appeal_Put(_citizenDashboard.RegistrationYear, _citizenDashboard.GrievanceId, Entry_Email.Text, DateOfComplaint, DateOfDecision, IntermediaryID, Entry_IntermediaryName.Text, Entry_URLIntermediary.Text,
                Entry_EmailIntermediary.Text, Editor_ContactDetails_GO_Intermediary.Text, Editor_Complaint.Text, Editor_Keyword.Text, Editor_Justification.Text, Editor_Relief_Sought.Text);
            
            Loading_activity.IsVisible = false;
        }
        async void Btn_Save_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = new Models.HitService();
            string IntermediaryID = "0";
            if (!await App.ValidateEmail(Entry_Email.Text, "Your Email Address"))
            {
                Loading_activity.IsVisible = false;
                return;
            }
            if (!await App.ValidateAddress(Entry_IntermediaryName.Text, "Name of the respondent intermediary against whom violation of the rules is alleged"))
            {
                Loading_activity.IsVisible = false;
                return;
            }
            if (!await App.ValidateEmail(Entry_EmailIntermediary.Text, "Email (Intermediary)"))
            {
                Loading_activity.IsVisible = false;
                return;
            }
            else
            {
                try
                {
                    IntermediaryID = List_IntermediaryMaster.FindAll(x => x.IntermediaryTitle.ToLower() == Entry_IntermediaryName.Text.ToLower()).First().IntermediaryId;
                }
                catch (Exception)
                {
                    IntermediaryID = "0";
                }
            }
            if (string.IsNullOrEmpty(Editor_Complaint.Text))
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, $"{App.LableValueGeneric("Mobile_Alert_ProvideInput")} '{App.LableValueGeneric("BriefofComplaintEdit")}'", App.Btn_OK);
                Editor_Complaint.Focus();

                return;
            }
            if (string.IsNullOrEmpty(Editor_Keyword.Text))
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, $"{App.LableValueGeneric("Mobile_Alert_ProvideInput")} '{App.LableValueGeneric("SubjectKeyword")}'", App.Btn_OK);
                Editor_Keyword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Editor_Justification.Text))
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, $"{App.LableValueGeneric("Mobile_Alert_ProvideInput")} '{App.LableValueGeneric("Justificationforappeal")}'", App.Btn_OK);
                Editor_Justification.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Editor_Relief_Sought.Text))
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, $"{App.LableValueGeneric("Mobile_Alert_ProvideInput")} '{App.LableValueGeneric("ReliefSoughtDetails")}'", App.Btn_OK);
                Editor_Relief_Sought.Focus();
                return;
            }
            Loading_activity.IsVisible = true;
            string DateOfComplaint, DateOfDecision;
            if (User_DateOfComplaint.IsVisible)
            {
                DateOfComplaint = "";
            }
            else
            {
                DateOfComplaint = DatePicker_User_DateOfComplaint.Date.ToString("yyyy-MM-dd");
            }
            if (User_DateOfDecision.IsVisible)
            {
                DateOfDecision = "";
            }
            else
            {
                DateOfDecision = DatePicker_User_DateOfDecision.Date.ToString("yyyy-MM-dd");
            }
            if (DatePicker_User_DateOfComplaint.Date > DatePicker_User_DateOfDecision.Date)
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ShouldbeAfter"), App.Btn_OK);
                return;
            }
            if (!DetailedList_CopyOfComplaint.IsVisible)
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_UploadCoC"), App.Btn_OK);
                return;
            }
            var MyResponce = await service.Appeal_Put(_citizenDashboard.RegistrationYear, _citizenDashboard.GrievanceId, Entry_Email.Text, DateOfComplaint, DateOfDecision, IntermediaryID, Entry_IntermediaryName.Text, Entry_URLIntermediary.Text,
                Entry_EmailIntermediary.Text, Editor_ContactDetails_GO_Intermediary.Text, Editor_Complaint.Text, Editor_Keyword.Text, Editor_Justification.Text, Editor_Relief_Sought.Text);
            if (MyResponce == 200)
            {
                await service.CitizenDashboard_Get(User_Mobile.Text);
                _citizenDashboard =  App.listCitizenDashboard.FindAll(x => x.GrievanceId == _citizenDashboard.GrievanceId).First();
                await Navigation.PushAsync(new AppealPreviewPage(_citizenDashboard));
            }
            Loading_activity.IsVisible = false;
        }
        async void Upload_borwser_Navigated(System.Object sender, Xamarin.Forms.WebNavigatedEventArgs e)
        {
            if (e.Url.StartsWith($"{App.MobileUpload_BaseURL_}/responseFromUpload?statusCode="))
            {
                //http://10.146.2.79/GAC/Public/MobileUpload/responseFromUpload?statusCode=eCX8cQgOZ28Dkf2KYQHXBg%3D%3D&statusMessage=zzZ9xE4Sbc19oZouqLcGkA%3D%3D&path=YJg3%2BA4QFGpPqR8xUNp3U06xMytiuPF3zZ%2BX0XbiDUo%3D&fileType=z7qiROJUUtJiOyZ9aPSsyw%3D%3D
                Uri uri = new Uri(e.Url);
                string statusCode = AESCryptography.DecryptAES(HttpUtility.ParseQueryString(uri.Query).Get("statusCode"));
                string statusMessage = AESCryptography.DecryptAES(HttpUtility.ParseQueryString(uri.Query).Get("statusMessage"));
                string path = AESCryptography.DecryptAES(HttpUtility.ParseQueryString(uri.Query).Get("path"));
                string Type = AESCryptography.DecryptAES(HttpUtility.ParseQueryString(uri.Query).Get("fileType"));
                if ((statusCode ?? "") != "200")
                {
                    await DisplayAlert(App.AppName, statusMessage, App.Btn_OK);
                    Upload_activity.IsVisible = false;
                    Upload_borwser.Source = $"{App.MobileUpload_BaseURL_}?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}&FileType={HttpUtility.UrlEncode(AESCryptography.EncryptAES(FileType))}&Mobile={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.UserMobile))}";
                }
                else
                {
                    var service = new Models.HitService();
                    await service.AppealDocument_Get(_citizenDashboard.UserMobile);
                    FillPage();
                    Upload_activity.IsVisible = false;
                }
                Upload_activity.IsVisible = false;
            }
        }
        
        void Btn_CloseBrowser_Clicked(System.Object sender, System.EventArgs e)
        {
            Upload_activity.IsVisible = false;
        }

        async void CC_Tapped(System.Object sender, System.EventArgs e)
        {
            try
            {
                var _FilePath = List_AppealDocument.FindAll(x => x.DocumentType == "CC").First().FilePath;
                var _FileExtension = List_AppealDocument.FindAll(x => x.DocumentType == "CC").First().FilePath.Split('.').Last();
                await Launcher.OpenAsync($"{App.MobileUpload_BaseURL_}/DownloadDocument?FilePath={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_FilePath))}&FileType={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_FileExtension))}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Failed To Open Attached", ex.Message, App.Btn_OK);
            }
        }
        async void CD_Tapped(System.Object sender, System.EventArgs e)
        {
            try
            {
                var _FilePath = List_AppealDocument.FindAll(x => x.DocumentType == "CD").First().FilePath;
                var _FileExtension = List_AppealDocument.FindAll(x => x.DocumentType == "CD").First().FilePath.Split('.').Last();
                await Launcher.OpenAsync($"{App.MobileUpload_BaseURL_}/DownloadDocument?FilePath={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_FilePath))}&FileType={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_FileExtension))}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Failed To Open Attached", ex.Message, App.Btn_OK);
            }
        }
        async void Btn_DeleteCC_Clicked(System.Object sender, System.EventArgs e)
        {
            bool Action = await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_DeleteFile"), App.LableValueGeneric("Yes"), App.LableValueGeneric("No"));
            if (Action)
            {
                ImageButton button = (ImageButton)sender;
                string FileId = button.CommandParameter.ToString();
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                var attachedURLs = List_AppealDocument.FindAll(x => x.DocumentType == "CC").ToList();
                int indexer = attachedURLs.FindIndex(x => x.FileId == FileId);
                if (indexer != -1)
                {
                    Upload_borwser.Source = $"{App.MobileUpload_BaseURL_}/DeleteFile?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(List_AppealDocument.FindAll(x => x.DocumentType == "CC" && x.FileId == FileId).ToList().First().RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(List_AppealDocument.FindAll(x => x.DocumentType == "CC" && x.FileId == FileId).ToList().First().GrievanceId))}&FileID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(FileId))}";
                }
                await Task.Delay(3000);
                await service.AppealDocument_Get(_citizenDashboard.UserMobile);
                await Task.Delay(500);
                FillPage();
                Upload_activity.IsVisible = false;
                Loading_activity.IsVisible = false;
            }
        }
        async void Btn_DeleteCD_Clicked(System.Object sender, System.EventArgs e)
        {
            bool Action = await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_DeleteFile"), App.LableValueGeneric("Yes"), App.LableValueGeneric("No"));
            if (Action)
            {
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                var attachedURLs = List_AppealDocument.FindAll(x => x.DocumentType == "CD").ToList();
                Upload_borwser.Source = $"{App.MobileUpload_BaseURL_}/DeleteFile?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(List_AppealDocument.FindAll(x => x.DocumentType == "CD").ToList().First().RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(List_AppealDocument.FindAll(x => x.DocumentType == "CD").ToList().First().GrievanceId))}&FileID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(List_AppealDocument.FindAll(x => x.DocumentType == "CD").ToList().First().FileId))}";
                await Task.Delay(3000);
                await service.AppealDocument_Get(_citizenDashboard.UserMobile);
                await Task.Delay(500);
                FillPage();
                Upload_activity.IsVisible = false;
                Loading_activity.IsVisible = false;
            }
        }

        void FillAppeal()
        {
            Entry_FName.IsEnabled = true;
            Entry_MName.IsEnabled = true;
            Entry_LName.IsEnabled = true;
            User_Mobile.IsEnabled = true;

            Entry_FName.Text = _citizenDashboard.FirstName;
            Entry_MName.Text = _citizenDashboard.MiddleName;
            Entry_LName.Text = _citizenDashboard.LastName;

            User_Mobile.Text = _citizenDashboard.UserMobile;
            Entry_Email.Text = _citizenDashboard.UserEmail;

            User_Mobile.IsEnabled = false;
            Entry_FName.IsEnabled = false;
            Entry_MName.IsEnabled = false;
            Entry_LName.IsEnabled = false;

            Entry_IntermediaryName.Text = _citizenDashboard.IntermediaryTitle;
            Entry_URLIntermediary.Text = _citizenDashboard.IntermediaryURL;
            Entry_EmailIntermediary.Text = _citizenDashboard.IntermediaryGROEmail;
            Entry_URLIntermediary.Text = _citizenDashboard.IntermediaryURL;
            Editor_ContactDetails_GO_Intermediary.Text = _citizenDashboard.IntermediaryAddress;
            Intermediary_Hint.IsVisible = false;

            Editor_Complaint.Text = _citizenDashboard.BriefofComplaint;
            Editor_Keyword.Text = _citizenDashboard.Keyword;
            Editor_Relief_Sought.Text = _citizenDashboard.RelieftSoughtSpecification;
            Editor_Justification.Text = _citizenDashboard.Justification;
            try
            {
                if (!string.IsNullOrEmpty(_citizenDashboard.dateofComplaint))
                {
                    User_DateOfComplaint.IsVisible = false;
                    DatePicker_User_DateOfComplaint.IsVisible = true;
                    Device.BeginInvokeOnMainThread(() => {
                        DatePicker_User_DateOfComplaint.Date = DateTime.Parse(_citizenDashboard.dateofComplaint).Date;
                    });
                }
            }
            catch (Exception)
            {

            }
            try
            {
                if (!string.IsNullOrEmpty(_citizenDashboard.dateofDecision))
                {
                    User_DateOfDecision.IsVisible = false;
                    DatePicker_User_DateOfDecision.IsVisible = true;
                    Device.BeginInvokeOnMainThread(() => {
                        DatePicker_User_DateOfDecision.Date = DateTime.Parse(_citizenDashboard.dateofDecision).Date;
                    });
                }
            }
            catch (Exception)
            {

            }

            FillPage();
        }
        void FillPage()
        {
            AppealDocumentDatabase appealDocumentDatabase = new AppealDocumentDatabase();
            List_AppealDocument = appealDocumentDatabase.FillAppealDocument(_citizenDashboard.RegistrationYear, _citizenDashboard.GrievanceId).ToList();
            if (List_AppealDocument.Any())
            {
                if (List_AppealDocument.FindIndex(x => x.DocumentType == "CC") != -1)
                {
                    var List_CC = List_AppealDocument.FindAll(x => x.DocumentType == "CC").ToList();
                    DetailedList_CopyOfComplaint.ItemsSource = List_CC;
                    DetailedList_CopyOfComplaint.HeightRequest = List_CC.Count() * 40;
                    DetailedList_CopyOfComplaint.IsVisible = true;
                    if (List_CC.Count() >= 3)
                    {
                        Btn_ChooseCompyOfComplaint.IsVisible = false;
                    }
                    else
                    {
                        Btn_ChooseCompyOfComplaint.IsVisible = true;
                    }
                }
                else
                {
                    DetailedList_CopyOfComplaint.IsVisible = false;
                    Btn_ChooseCompyOfComplaint.IsVisible = true;
                }
                if (List_AppealDocument.FindIndex(x => x.DocumentType == "CD") != -1)
                {
                    Link_CD.Text = List_AppealDocument.FindAll(x => x.DocumentType == "CD").First().FileName;
                    Link_CD.IsVisible = true;
                    Btn_DeleteCD.IsVisible = true;
                    Btn_ChooseCompyOfDecision.IsVisible = false;
                }
                else
                {
                    Link_CD.IsVisible = false;
                    Btn_DeleteCD.IsVisible = false;
                    Btn_ChooseCompyOfDecision.IsVisible = true;
                }
            }
            else
            {
                DetailedList_CopyOfComplaint.IsVisible = false;
                Link_CD.IsVisible = false;
                Btn_DeleteCD.IsVisible = false;
            }
        }

        void Keyword_Hint_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var current = e.Item as Groundkeywords;
            try
            {
                var Word = Editor_Keyword.Text.Substring(Editor_Keyword.Text.LastIndexOf(' ') + 1);
                var TextAfterToReplace = Editor_Keyword.Text.Substring(0,Editor_Keyword.Text.Length- Word.Length);
                var FinalText = (TextAfterToReplace + current.Keyword).Replace("  ", "");
                Editor_Keyword.Text = FinalText + " ";
            }
            catch (Exception ex)
            {

            }
        }

        void Editor_Keyword_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Editor_Keyword.Text))
                {
                    Keyword_Hint.IsVisible = false;
                }
                else
                {
                    if (Editor_Keyword.Text.Length > 0)
                    {
                        string Word = "";
                        IEnumerable<Groundkeywords> ListToBeFill;
                        GroundkeywordsDatabase groundkeywordsDatabase = new GroundkeywordsDatabase();
                        if (Editor_Keyword.Text.Contains(" "))
                        {
                            Word = Editor_Keyword.Text.Substring(Editor_Keyword.Text.LastIndexOf(' ') + 1);
                            if (string.IsNullOrEmpty(Word))
                            {
                                Word = "dfbvfdagvsdfgvafsdgvdfsagvdfsgvafsda";
                            }
                            ListToBeFill = groundkeywordsDatabase.FillGroundkeywords(Word);
                        }
                        else
                        {
                            ListToBeFill = groundkeywordsDatabase.FillGroundkeywords(Editor_Keyword.Text);
                        }
                        if (ListToBeFill.Count() == 0)
                        {
                            Keyword_Hint.IsVisible = false;
                        }
                        else
                        {
                            Keyword_Hint.ItemsSource = ListToBeFill;
                            Keyword_Hint.IsVisible = true;
                        }
                    }
                    else
                    {
                        Keyword_Hint.IsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Keyword_Hint.IsVisible = false;
            }
        }

        void DatePicker_User_DateOfComplaint_DateSelected(System.Object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            DatePicker_User_DateOfDecision.MinimumDate = DatePicker_User_DateOfComplaint.Date;
            //User_DateOfComplaint.IsReadOnly = true;
        }

        void DatePicker_User_DateOfDecision_DateSelected(System.Object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            DatePicker_User_DateOfComplaint.MaximumDate = DatePicker_User_DateOfDecision.Date;
            //User_DateOfDecision.IsReadOnly = true;
        }

        async void Editor_Justification_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //await scrollView.ScrollToAsync(Entry_URLIntermediary.X, Entry_URLIntermediary.Y, true);
            string[] words = Editor_Justification.Text.Split(' ');
            if (words.Count() >= 251)
            {
                Lbl_JustificationWord.Text.Remove(Lbl_JustificationWord.Text.Length - 1, 1);
                await DisplayAlert(App.AppName, App.LableValueGeneric("Max250JustificationValidation"), App.Btn_OK);
                return;
            }
            if (Editor_Justification.Text.Length < 1)
            {
                Lbl_JustificationWord.Text = $"0 word(s)/ 250";
            }
            else
            {
                Lbl_JustificationWord.Text = $"{words.Count()} word(s)/ 250";
            }
        }
        async void DetailedList_CopyOfComplaint_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {

            var currentRecord = e.Item as AppealDocument;
            try
            {
                var _Extension = currentRecord.FilePath.Split('.').Last();
                await Launcher.OpenAsync($"{App.MobileUpload_BaseURL_}/DownloadDocument?FilePath={HttpUtility.UrlEncode(AESCryptography.EncryptAES(currentRecord.FilePath))}&FileType={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_Extension))}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Failed To Open Attached", ex.Message, App.Btn_OK);
            }
        }

        void Editor_Keyword_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Keyword_Hint.IsVisible = false;
        }

        void Entry_URLIntermediary_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
        }

        async void Entry_EmailIntermediary_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            await scrollView.ScrollToAsync(Entry_EmailIntermediary, ScrollToPosition.End, true);
        }

        void Editor_ContactDetails_GO_Intermediary_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
        }

        void Editor_Complaint_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
        }

        void Editor_Justification_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
        }

        async void Editor_Relief_Sought_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {

            //await scrollView.ScrollToAsync(Editor_Relief_Sought, ScrollToPosition.End, true);
            ////ScrollToPosition(Editor_Relief_Sought.TranslationX, Editor_Relief_Sought.TranslationY);
        }

        void User_DateOfComplaint_Tapped(System.Object sender, System.EventArgs e)
        {
            User_DateOfComplaint.IsVisible = false;
            Device.BeginInvokeOnMainThread(() => {
                DatePicker_User_DateOfComplaint.IsVisible = true;
                DatePicker_User_DateOfComplaint.Focus();
            });
        }
        void User_DateOfDecision_Tapped(System.Object sender, System.EventArgs e)
        {
            User_DateOfDecision.IsVisible = false;
            Device.BeginInvokeOnMainThread(() => {
                DatePicker_User_DateOfDecision.IsVisible = true;
                DatePicker_User_DateOfDecision.Focus();
            });
        }
    }
}

