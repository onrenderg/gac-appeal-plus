using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GACAppeal
{	
	public partial class DisposedPage : ContentPage
	{	
		public DisposedPage ()
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            Lbl_Title.Text = $"Appeals - Disposed of\n{App.listCitizenProfile.First().FirstName} {App.listCitizenProfile.First().MiddleName} {App.listCitizenProfile.First().LastName} ({App.listCitizenProfile.First().UserMobile})";// \n{App.listCitizenProfile.First().UserMobile}
            CitizenDashboardDatabase citizenDashboardDatabase = new CitizenDashboardDatabase();
            var MyAppeals = citizenDashboardDatabase.GetCitizenDashboard("Select *,strftime('%d/%m/%Y', IntermediaryDate)Extra1,(case GrievnaceStatus when '0' then 'Draft_c_ico.png' when '1' then 'Draft_c_ico.png' when '2' then 'Submitted_c_ico.png' when '7' then 'Submitted_c_ico.png' when '4' then 'Query_c_ico.png' when '5' then 'Rejected_c_ico.png' when '6' then 'Disposed_c_ico.png' END)ImageToDisplay from CitizenDashboard where GrievnaceStatus = '6';", new string[0]).ToList();
            if (MyAppeals.Any())
            {
                MyApeals_List.IsVisible = true;
                MyApeals_List.ItemsSource = MyAppeals;
                MyApeals_List.HeightRequest = 45 * MyAppeals.Count();
                Lbl_NoRecord.IsVisible = false;
            }
            else
            {
                MyApeals_List.IsVisible = false;
                Lbl_NoRecord.Text = "No Disposed Appeal Found";
                Lbl_NoRecord.IsVisible = true;
            }
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
            App.LableValue(Lbl_NoRecord, "NoRecordFound");
            Lbl_Title.Text = $"{App.LableValueGeneric("Appeal")} - {App.LableValueGeneric("Disposedof")}\n{App.listCitizenProfile.First().FirstName} {App.listCitizenProfile.First().MiddleName} {App.listCitizenProfile.First().LastName} ({App.listCitizenProfile.First().UserMobile})";// \n{App.listCitizenProfile.First().UserMobile}
            Page_name.Title = App.LableValueGeneric("Disposedof");
        }
        async void MyApeals_List_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var current = e.Item as CitizenDashboard;
            string MyOption = "";
            MyOption = await DisplayActionSheet(current.IntermediaryTitle, App.LableValueGeneric("Cancel"), null, App.LableValueGeneric("AppealPreview"), App.LableValueGeneric("Decision"), App.LableValueGeneric("ViewCompliance"));
            if (MyOption == App.LableValueGeneric("AppealPreview"))
            {
                await Launcher.OpenAsync($"{App.MobileUpload_BaseURL_}/AppealPDF?RegistrationYear={HttpUtility.UrlEncode(AESCryptography.EncryptAES(current.RegistrationYear))}&GrievanceID={HttpUtility.UrlEncode(AESCryptography.EncryptAES(current.GrievanceId))}");
            }
            else if (MyOption == App.LableValueGeneric("Decision"))
            {
                await Launcher.OpenAsync($"{App.MobileUpload_BaseURL_}/DownloadDocument?FilePath={HttpUtility.UrlEncode(AESCryptography.EncryptAES(current.DecisionFilePath))}&FileType={HttpUtility.UrlEncode(AESCryptography.EncryptAES("pdf"))}");
            }
            else if (MyOption == App.LableValueGeneric("ViewCompliance"))
            {
                await Launcher.OpenAsync(current.ComplianceURL);
            }
            //Navigation.PushAsync(new AppealPreviewPage(current));
        }
	}
}

