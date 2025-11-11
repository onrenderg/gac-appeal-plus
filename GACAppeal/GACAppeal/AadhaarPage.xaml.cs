using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GACAppeal
{
    public partial class AadhaarPage : ContentPage
    {
        public AadhaarPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                await service.DashboardCount_Get();
                await service.RefreshMasters_Get();
                Loading_activity.IsVisible = false;
                if (App.list_DashboardCount.Any())
                {
                    Lbl_Appeals_Received_Count.Text = App.list_DashboardCount.First().Submitted;
                    Lbl_Disposed_Of_Count.Text = App.list_DashboardCount.First().Disposed;
                }
                App.update();
            });
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
            //App.LableValue(Lbl_OneTime, "GrievanceAppellateCommittee");
            App.LableValue(Lbl_AadhaarNo, "AadhaarNumber");
            App.LableValue(Lbl_NameInAadhaar, "NameasinAadhaar");
            App.LableValue(Lbl_OTP, "OTP");
            App.LableValue(Lbl_EULAadhaar, "AadhaarConsent");
            App.LableValue(Lbl_Appeals_Received, "AppealsReceived");
            App.LableValue(Lbl_Disposed_Of, "Disposedof");
            App.LableValue(Lbl_PleaseWait, "Mobile_PleaseWait");
            Entry_AadhaarNumber.Placeholder = App.LableValueGeneric("EnterYourAadhaarNumber");
            Entry_AadhaarNumberName.Placeholder = App.LableValueGeneric("EnterYourNameasinAadhaar");
            Btn_Verify.Text = App.LableValueGeneric("Verify");
            Btn_Cancel.Text = App.LableValueGeneric("Cancel");
        }
        async void Btn_Verify_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!Stack_OTP.IsVisible)
            {
                if (!Chk_EULA.IsChecked)
                {
                    await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_Consent"), App.Btn_OK);
                    return;
                }
                if (string.IsNullOrEmpty(Entry_AadhaarNumber.Text))
                {
                    await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_Valid"), App.Btn_OK);
                    return;
                }
                else if (string.IsNullOrEmpty((Entry_AadhaarNumberName.Text ?? "").Trim()))
                {
                    await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_Name"), App.Btn_OK);
                    return;
                }

                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                var MyNameVerification = await service.AadharNameAndNumberVerification_Post(Entry_AadhaarNumber.Text, Entry_AadhaarNumberName.Text);
                if (MyNameVerification != 200)
                {
                    Loading_activity.IsVisible = false;
                    
                    return;
                }
                var MyResponse = await service.AadharVerification_Post(Entry_AadhaarNumber.Text);
                if (Preferences.Get("AadhaarVerified", "N") == "M")
                {
                    Entry_AadhaarNumber.IsReadOnly = true;
                    if (!string.IsNullOrEmpty((Entry_AadhaarNumberName.Text ?? "").Trim()))
                    {
                        string[] Detect_Name = (Entry_AadhaarNumberName.Text ?? "").Trim().Split();
                        Entry_AadhaarNumberName.IsReadOnly = true;
                        if (Detect_Name.Length == 1)
                        {
                            Preferences.Set("FirstName", Detect_Name[0]);
                        }
                        else if (Detect_Name.Length == 2)
                        {
                            Preferences.Set("FirstName", Detect_Name[0]);
                            Preferences.Set("LastName", Detect_Name[1]);
                        }
                        else if (Detect_Name.Length >= 3)
                        {
                            Preferences.Set("FirstName", Detect_Name[0]);
                            Preferences.Set("MiddleName", Detect_Name[1]);
                            Preferences.Set("LastName", Detect_Name[2]);
                        }
                        else
                        {
                            Entry_AadhaarNumberName.IsReadOnly = false;
                            await DisplayAlert(App.AppName, App.LableValueGeneric("Mobile_Alert_Aadhaar_Name"), App.Btn_OK);
                            return;
                        }
                    }
                    Stack_OTP.IsVisible = true;
                    Stack_EULA.IsVisible = false;
                    //App.Current.MainPage = new NavigationPage(new HomePage());
                }
                Loading_activity.IsVisible = false;
            }
            else
            {
                if (string.IsNullOrEmpty(Entry_AadhaarOTP.Text))
                {
                    await DisplayAlert(App.AppName, "Please Enter OTP", App.Btn_OK);
                    return;
                }
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                var MyResponse = await service.AadharVerification_Put(Entry_AadhaarNumber.Text,Entry_AadhaarOTP.Text);
                if (Preferences.Get("AadhaarVerified", "N") == "Y")
                {
                    var response = await service.AadharVerification_Post(App.listCitizenProfile.First().UserId, Preferences.Get("FirstName", ""), Preferences.Get("MiddleName", ""), Preferences.Get("LastName", ""));
                    if (response == 201 || response == 200)
                    {
                        await service.CitizenProfile_Get(Preferences.Get("Mobile_No", ""));
                        App.Current.MainPage = new NavigationPage(new HomePage());
                    }
                    else
                    {
                        Preferences.Set("AadhaarVerified", "M");
                    }
                }
                Loading_activity.IsVisible = false;
            }
        }

        void Btn_Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}

