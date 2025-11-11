using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace GACAppeal
{	
	public partial class TransferredPage : ContentPage
	{	
		public TransferredPage ()
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            Lbl_Title.Text = $"Transferred\n{App.listCitizenProfile.First().FirstName} {App.listCitizenProfile.First().MiddleName} {App.listCitizenProfile.First().LastName} ({App.listCitizenProfile.First().UserMobile})";// \n{App.listCitizenProfile.First().UserMobile}
            CitizenDashboardDatabase citizenDashboardDatabase = new CitizenDashboardDatabase();
            var MyAppeals = citizenDashboardDatabase.GetCitizenDashboard("Select *,strftime('%d/%m/%Y', IntermediaryDate)Extra1,(case GrievnaceStatus when '0' then 'Draft_c_ico.png' when '1' then 'Draft_c_ico.png' when '2' then 'Submitted_c_ico.png' when '7' then 'Submitted_c_ico.png' when '4' then 'Query_c_ico.png' when '5' then 'Rejected_c_ico.png' when '6' then 'Disposed_c_ico.png' END)ImageToDisplay from CitizenDashboard where GrievnaceStatus = '4';", new string[0]).ToList();
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
                Lbl_NoRecord.Text = "No Appeal with Transferred";
                Lbl_NoRecord.IsVisible = true;
            }
            FillLable();
        }
        //protected override void OnAppearing()
        //{
        //    FillLable();
        //}
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
            //await DisplayAlert(App.AppName, "Feature Not Ready", App.Btn_OK);
            //Navigation.PushAsync(new AppealPreviewPage(current.RegistrationYear, current.GrievanceId));
        }
	}
}

