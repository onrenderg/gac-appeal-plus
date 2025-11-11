using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GACAppeal
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            Preferences.Set("AadhaarVerified", "Y");
            Device.BeginInvokeOnMainThread(async () =>
            {
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                await service.CitizenProfile_Get(Preferences.Get("Mobile_No", ""));
                await service.CitizenDashboard_Get(Preferences.Get("Mobile_No", ""));
                await service.AppealDocument_Get(Preferences.Get("Mobile_No", ""));
                await service.RefreshMasters_Get();
                
                //await service.IntermediaryMaster_Get();
                //await service.Groundkeywords_Get();
                //await service.LanguageMaster_Get();
                //await service.FormLabels_Get();
                if (App.listCitizenProfile.Any())
                {
                    Lbl_Title.Text = $"{App.LableValueGeneric("Dashboard")}\n{App.listCitizenProfile.First().FirstName} {App.listCitizenProfile.First().MiddleName} {App.listCitizenProfile.First().LastName} ({App.listCitizenProfile.First().UserMobile})";
                }
                else
                {
                    Lbl_Title.Text = $"{App.LableValueGeneric("Dashboard")}\n({Preferences.Get("Mobile_No", "")})";
                }
                App.update();
                Loading_activity.IsVisible = false;
                FillHome();
                FillLable();
                
                Loading_activity.IsVisible = false;
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
            if (App.listCitizenProfile.Any())
            {
                Lbl_Title.Text = $"{App.LableValueGeneric("Dashboard")}\n{App.listCitizenProfile.First().FirstName} {App.listCitizenProfile.First().MiddleName} {App.listCitizenProfile.First().LastName} ({App.listCitizenProfile.First().UserMobile})";
            }
            else
            {
                Lbl_Title.Text = $"{App.LableValueGeneric("Dashboard")}\n({Preferences.Get("Mobile_No", "")})";
            }
            App.LableValue(Lbl_Draft_lbl, "Draft");
            App.LableValue(Lbl_Submitted_lbl, "UnderProcess");
            App.LableValue(Lbl_Rejected_lbl, "NotAdmitted");
            App.LableValue(Lbl_Disposed_lbl, "Disposedof");
            App.LableValue(Lbl_Total_lbl, "Total");
            Btn_AddNew.Text = App.LableValueGeneric("FileNewAppeal");
        }
        void FillHome()
        {
            CitizenDashboardDatabase citizenDashboardDatabase = new CitizenDashboardDatabase();
            Lbl_Total.Text = citizenDashboardDatabase.GetCitizenDashboard("Select count(*)cnt_total from CitizenDashboard", new string[0]).First().cnt_total;

            Lbl_Draft.Text = citizenDashboardDatabase.GetCitizenDashboard("Select count(*)cnt_draft from CitizenDashboard where GrievnaceStatus  in ('0','1');", new string[0]).First().cnt_draft;

            Lbl_Submitted.Text = citizenDashboardDatabase.GetCitizenDashboard("Select count(*)cnt_submitted from CitizenDashboard where GrievnaceStatus  in ('2','7');", new string[0]).First().cnt_submitted;

            Lbl_Rejected.Text = citizenDashboardDatabase.GetCitizenDashboard("Select count(*)cnt_rejected from CitizenDashboard where GrievnaceStatus  = '5';", new string[0]).First().cnt_rejected;

            //Lbl_Query.Text = citizenDashboardDatabase.GetCitizenDashboard("Select count(*)cnt_queryraised from CitizenDashboard where GrievnaceStatus  = '4';", new string[0]).First().cnt_queryraised;

            Lbl_Disposed.Text = citizenDashboardDatabase.GetCitizenDashboard("Select count(*)cnt_disposed from CitizenDashboard where GrievnaceStatus  = '6';", new string[0]).First().cnt_disposed;

            var MyAppeals = citizenDashboardDatabase.GetCitizenDashboard("Select *,strftime('%d/%m/%Y', IntermediaryDate)Extra1,(case GrievnaceStatus when '0' then 'Draft_c_ico.png' when '1' then 'Draft_c_ico.png' when '2' then 'Submitted_c_ico.png' when '3' then 'Submitted_c_ico.png' when '4' then 'Query_c_ico.png' when '5' then 'Rejected_c_ico.png' when '6' then 'Disposed_c_png' END)ImageToDisplay from CitizenDashboard;", new string[0]).ToList();
            MyApeals_List.ItemsSource = MyAppeals;
            MyApeals_List.HeightRequest = 45 * MyAppeals.Count();
        }
        async void Btn_AddNew_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = new Models.HitService();
            var responce = await service.Appeal_Post(App.listCitizenProfile.First().UserMobile, App.listCitizenProfile.First().UserId, App.listCitizenProfile.First().UserProfilleId, App.listCitizenProfile.First().FirstName, App.listCitizenProfile.First().MiddleName, App.listCitizenProfile.First().LastName, App.listCitizenProfile.First().UserEmail);
            if (responce == 201 || responce == 200)
            {
                await service.CitizenDashboard_Get(Preferences.Get("Mobile_No", ""));
                FillHome();
                CitizenDashboardDatabase citizenDashboardDatabase = new CitizenDashboardDatabase();
                var MyItem =  citizenDashboardDatabase.FillCitizenDashboardByID(AESCryptography.DecryptAES(Preferences.Get("GrievanceID","")));
                if (MyItem.Any())
                {
                    await Navigation.PushAsync(new AppealPage(MyItem.First()));
                }
            }
        }
        void Total_Tapped(System.Object sender, System.EventArgs e)
        {
            if (Lbl_Total.Text == "0")
            {
                return;
            }
            Navigation.PushAsync(new AppealsTabbedPage(0));
        }
        void Draft_Tapped(System.Object sender, System.EventArgs e)
        {
            if (Lbl_Draft.Text == "0")
            {
                return;
            }
            Navigation.PushAsync(new AppealsTabbedPage(1));
        }
        
        void UnderProcess_Tapped(System.Object sender, System.EventArgs e)
        {
            if (Lbl_Submitted.Text == "0")
            {
                return;
            }
            Navigation.PushAsync(new AppealsTabbedPage(2));
        }
        void NotSubmitted_Tapped(System.Object sender, System.EventArgs e)
        {
            if (Lbl_Rejected.Text == "0")
            {
                return;
            }
            Navigation.PushAsync(new AppealsTabbedPage(3));
        }
        
        void Disposed_Tapped(System.Object sender, System.EventArgs e)
        {
            if (Lbl_Disposed.Text == "0")
            {
                return;
            }
            Navigation.PushAsync(new AppealsTabbedPage(4));
        }
        

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            bool action = await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Logout"), App.LableValueGeneric("Yes"), App.LableValueGeneric("No"));
            if (action)
            {
                CitizenProfileDatabase citizenProfileDatabase = new CitizenProfileDatabase();
                citizenProfileDatabase.DeleteCitizenProfile();
                citizenProfileDatabase.FillCitizenProfile();
                App.Current.MainPage = new NavigationPage(new MainPage());
            }
        }

        void Language_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new LanguagePage());
        }

        void MyApeals_List_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
        }
    }
}

