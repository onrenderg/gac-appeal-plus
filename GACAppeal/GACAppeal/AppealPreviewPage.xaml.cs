using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GACAppeal
{
    public partial class AppealPreviewPage : ContentPage
    {
        List<AppealDocument> List_AppealDocument;
        CitizenDashboard _citizenDashboard;
        public AppealPreviewPage(CitizenDashboard citizenDashboard)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            _citizenDashboard = citizenDashboard;
            FillPage();
            FillLable();
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
        
            App.LableValue(Lbl_Appellant_Detail, "AppellantDetails");
            Lbl_Title.Text = $"{App.LableValueGeneric("AppealNumber")}: {_citizenDashboard.AppealID ?? ""}\n{App.LableValueGeneric("Status")}: {_citizenDashboard.StatusTitle ?? ""}";
            App.LableValue(Lbl_CitizenName_lbl, "Name");
            App.LableValue(Lbl_Mobile_lbl, "Mobile");
            App.LableValue(Lbl_Email_lbl, "Email");
            App.LableValue(Lbl_Respondent_Intermediary_Detail, "RespondentIntermediaryDetail");
            App.LableValue(Lbl_Appeal_Date_lbl, "AppealDate");
            App.LableValue(Lbl_Status_lbl, "Status", " *");
            App.LableValue(Lbl_DateOfComplaint_lbl, "DateofComplaintregisteredwiththeintermediary");
            App.LableValue(Lbl_DateOfDecision_lbl, "DateofDecisiongivenbytheintermediary");
            App.LableValue(Lbl_Name_Intermediary_lbl, "NameoftheRespondentIntermediary");
            App.LableValue(Lbl_URL_Intermediary_lbl, "URLoftheRespondentIntermediary");
            App.LableValue(Lbl_Email_Intermediary_lbl, "IntermediaryEmail");
            App.LableValue(Lbl_Address_Intermediary_lbl, "IntermediaryAddress");
            App.LableValue(Lbl_BriefOfComplaint_lbl, "BriefofComplaintPreview");
            App.LableValue(Lbl_Subject_lbl, "SubjectKeyword");
            App.LableValue(Lbl_Relief_Sought_lbl, "ReliefSoughtPreview");
            App.LableValue(Lbl_Justification_lbl, "JustificationPreview");
            App.LableValue(Lbl_Document_Uploaded, "DocumentUploaded");
            App.LableValue(Lbl_CopyOfComplaint, "CCIntermediary");
            App.LableValue(Lbl_AnyOther, "Anyotherrelevantinformation");
            App.LableValue(Lbl_EULA, "IherebySubmit", " *");
            Lbl_FinalSubmittedDateTime.Text = $"{App.LableValueGeneric("DateandTime")}: _____________ [{App.LableValueGeneric("Indianstandardtime")}]";
            Btn_Submit.Text = App.LableValueGeneric("Submit");
            Btn_DownloadPDF.Text = App.LableValueGeneric("DownloadPDF");

        }
        void Language_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new LanguagePage());
        }
        void FillPage()
        {
            AppealDocumentDatabase appealDocumentDatabase = new AppealDocumentDatabase();
            //AppealReliefMasterDatabase appealReliefMasterDatabase = new AppealReliefMasterDatabase();
            //var List_AppealReliefMaster = appealReliefMasterDatabase.FillAppealReliefMaster().ToList();
            List_AppealDocument = appealDocumentDatabase.FillAppealDocument(_citizenDashboard.RegistrationYear, _citizenDashboard.GrievanceId).ToList();
            Lbl_Title.Text = $"{App.LableValueGeneric("AppealNumber")}: {_citizenDashboard.AppealID ?? ""}\nStatus: {_citizenDashboard.StatusTitle ?? ""}";
            if (_citizenDashboard.GrievnaceStatus == "1")
            {
                Stack_EULA.IsVisible = true;
                Btn_Submit.IsVisible = true;
            }
            else
            {
                Stack_EULA.IsVisible = false;
                Btn_Submit.IsVisible = false;
            }
            Lbl_CitizenName.Text = $"{_citizenDashboard.FirstName} {_citizenDashboard.MiddleName} {_citizenDashboard.LastName}";
            Lbl_Mobile.Text = _citizenDashboard.UserMobile;
            Lbl_Email.Text = _citizenDashboard.UserEmail;
            try
            {
                Lbl_Appeal_Date.Text = DateTime.Parse(_citizenDashboard.ReceiptDate).ToString("dd-MM-yyyy");
            }
            catch (Exception ex)
            {
                Lbl_Appeal_Date.Text = _citizenDashboard.ReceiptDate;
            }

            try
            {
                Lbl_DateOfComplaint.Text = DateTime.Parse(_citizenDashboard.dateofComplaint).ToString("dd-MM-yyyy");
            }
            catch (Exception)
            {
                Lbl_DateOfComplaint.Text = _citizenDashboard.dateofComplaint;
            }
            try
            {
                Lbl_DateOfDecision.Text = DateTime.Parse(_citizenDashboard.dateofDecision).ToString("dd-MM-yyyy");
            }
            catch (Exception)
            {
                Lbl_DateOfDecision.Text = _citizenDashboard.dateofDecision;
            }
            Lbl_Status.Text = _citizenDashboard.StatusTitle;
            Lbl_Name_Intermediary.Text = _citizenDashboard.IntermediaryTitle ?? "";
            Lbl_URL_Intermediary.Text = _citizenDashboard.IntermediaryURL ?? "";
            Lbl_Email_Intermediary.Text = _citizenDashboard.IntermediaryGROEmail ?? "";
            Lbl_Address_Intermediary.Text = _citizenDashboard.IntermediaryAddress ?? "";
            Lbl_BriefOfComplaint.Text = _citizenDashboard.BriefofComplaint ?? "";
            Lbl_Subject.Text = _citizenDashboard.Keyword ?? "";
            Lbl_Relief_Sought.Text = _citizenDashboard.RelieftSoughtSpecification ?? "";
            Lbl_Justification.Text = _citizenDashboard.Justification ?? "";
            Lbl_SignName.Text = $"{_citizenDashboard.FirstName} {_citizenDashboard.MiddleName} {_citizenDashboard.LastName} [Name of Appellant]".Replace("  "," ");
            
            if (List_AppealDocument.FindIndex(x => x.DocumentType == "CC") != -1)
            {
                var List_CC = List_AppealDocument.FindAll(x => x.DocumentType == "CC").ToList();
                DetailedList_CopyOfComplaint.ItemsSource = List_CC;
                DetailedList_CopyOfComplaint.HeightRequest = List_CC.Count() * 40;
                DetailedList_CopyOfComplaint.IsVisible = true;
            }
            else
            {
                DetailedList_CopyOfComplaint.IsVisible = false;
            }
            if (List_AppealDocument.FindIndex(x => x.DocumentType == "CD") != -1)
            {
                Link_CD.Text = List_AppealDocument.FindAll(x => x.DocumentType == "CD").First().FileName;
                Link_CD.IsVisible = true;
            }
            else
            {
                Link_CD.IsVisible = false;
            }
            
        }
        async void Btn_SaveDraft_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!CheckBox_EULA.IsChecked)
            {
                await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ShowConsent"), App.Btn_OK);
                return;
            }
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }

        async void Btn_Submit_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = new Models.HitService();
            if (!await App.ValidateAddress(Lbl_Name_Intermediary.Text, "Name of the Intermediary"))
            {
                return;
            }
            if (string.IsNullOrEmpty(Lbl_URL_Intermediary.Text))
            {
                await DisplayAlert(App.AppName, "Intermediary URL is required", App.Btn_OK);
                return;
            }
            if (!CheckBox_EULA.IsChecked)
            {
                await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_ShowConsent"), App.Btn_OK);
                return;
            }
            Loading_activity.IsVisible = true;
            //var service = new Models.HitService();
            var ServiceResponse = await service.FinalAppeal(_citizenDashboard.RegistrationYear, _citizenDashboard.GrievanceId, _citizenDashboard.UserMobile, "");
            if (ServiceResponse == 200 || ServiceResponse == 201)
            {
                var webvi = new WebView();
                webvi.Source = $"{App.MobileUpload_BaseURL_}/sendMail?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}";
                App.Current.MainPage = new NavigationPage(new HomePage());
            }
            Loading_activity.IsVisible = false;
        }
        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new HomePage());
        }

        void Btn_DownloadPDF_Clicked(System.Object sender, System.EventArgs e)
        {
            var sds = $"{App.MobileUpload_BaseURL_}/AppealPDF?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}";
            //Console.WriteLine($"{App.MobileUpload_BaseURL_}/AppealPDF?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}");
            Launcher.OpenAsync($"{App.MobileUpload_BaseURL_}/AppealPDF?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(_citizenDashboard.GrievanceId))}");
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
    }
}

