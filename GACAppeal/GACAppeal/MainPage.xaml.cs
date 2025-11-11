using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static System.Net.WebRequestMethods;

namespace GACAppeal
{
    public partial class MainPage : ContentPage
    {
        int TimeInSecond = 60;
        public MainPage()
        {
            Preferences.Remove("Mobile_No");
            Preferences.Remove("AadhaarVerified");
            Models.RefreshMastersDatabase refreshMastersDatabase = new Models.RefreshMastersDatabase();
            CitizenProfileDatabase citizenProfileDatabase = new CitizenProfileDatabase();
            citizenProfileDatabase.DeleteCitizenProfile();
            citizenProfileDatabase.FillCitizenProfile();
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () =>
            {
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                

                await service.RefreshMasters_Get();
                await service.DashboardCount_Get();
                Loading_activity.IsVisible = false;
                FillLable();
                if (App.list_DashboardCount.Any())
                {
                    Lbl_Appeals_Received_Count.Text = App.list_DashboardCount.First().Submitted;
                    Lbl_Disposed_Of_Count.Text = App.list_DashboardCount.First().Disposed;
                }
                App.update();
            });
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
        }
        protected override void OnAppearing()
        {
            FillLable();
        }
        void FillLable()
        {
            App.AppName = App.LableValueGeneric("GrievanceAppellateCommittee");
            App.Btn_OK =  App.LableValueGeneric("Mobile_OK");
            App.Btn_Cancel = App.LableValueGeneric("Cancel");
            App.NoInternet_ = App.LableValueGeneric("Mobile_NoInternet");

            App.LableValue(Lbl_Nav_title, "GrievanceAppellateCommittee");
            App.LableValue(Lbl_AppDesc, "Mobile_AppDesc");
            App.LableValue(Lbl_mandatory, "Mobile_Mandatory");
            App.LableValue(Lbl_MobileNo, "EnterMobileNumber", " *");
            Entry_Mobile.Placeholder = App.LableValueGeneric("EnterMobileNumberPlaceHolder");
            Btn_GenerateOTP.Text = App.LableValueGeneric("SendOTP");
            App.LableValue(Lbl_EnterOTP, "EnterOTPSentonMobileNumber");
            Btn_Verify.Text = App.LableValueGeneric("Verify");
            App.LableValue(Lbl_Appeals_Received, "AppealsReceived");
            App.LableValue(Lbl_Disposed_Of, "Disposedof");
            App.LableValue(Lbl_PleaseWait, "Mobile_PleaseWait");
            img_logogac.Source = "logogac.png";
            if (Preferences.Get("LanguageCode", "en-IN") == "hi-IN")
            {
                img_logogac.Source = "logogac_hi.png";
            }
        }
        async void Btn_GenerateOTP_Clicked(System.Object sender, System.EventArgs e)
        {
            var m = App.LableValueGeneric("Mobile_ValidMobileNo");
            if (string.IsNullOrEmpty(Entry_Mobile.Text))
            {
                
                await DisplayAlert(App.AppName, m, App.Btn_OK);
                return;
            }
            try
            {
                if (Int64.Parse(Entry_Mobile.Text) < 6000000000)
                {
                    await DisplayAlert(App.AppName, m, App.Btn_OK);
                    return;
                }
            }
            catch (Exception)
            {
                await DisplayAlert(App.AppName, m, App.Btn_OK);
                return;
            }
            
            if (Btn_GenerateOTP.Text != App.LableValueGeneric("Mobile_ResendOTP"))
            {
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                if (await service.OTP_Post(Entry_Mobile.Text) == 201)
                {
                    Entry_Mobile.IsReadOnly = true; 
                    Stack_OTP.IsVisible = true;
                    Loading_activity.IsVisible = false;
                    Lbl_EnterOTP.Text= $"{App.LableValueGeneric("EnterOTPSentonMobileNumber")} '{Entry_Mobile.Text}' {App.LableValueGeneric("withOTPID")} '{Preferences.Get("otp_id", "xxxx")}'";
                    Entry_OTP.Placeholder = $"{Preferences.Get("otp_id", "xxxx")}";
                    Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                    {
                        TimeInSecond = TimeInSecond - 1;
                        if (TimeInSecond != 0)
                        {
                            Btn_GenerateOTP.Text = $"{App.LableValueGeneric("Mobile_ResendOTP")} {TimeInSecond} S";
                            return true;
                        }
                        else
                        {
                            TimeInSecond = 60;
                            Btn_GenerateOTP.Text = $"{App.LableValueGeneric("Mobile_ResendOTP")}";
                            return false;
                        }
                    });
                }
                Loading_activity.IsVisible = false;
            }
            Loading_activity.IsVisible = false;
        }

        async void Btn_Verify_Clicked(System.Object sender, System.EventArgs e)
        {
            Loading_activity.IsVisible = true;
            var service = new Models.HitService();
            if (await service.OTP_Put(Entry_Mobile.Text, Entry_OTP.Text) == 200)
            {
                Preferences.Set("Mobile_No", Entry_Mobile.Text);
                await service.CitizenProfile_Get(Preferences.Get("Mobile_No", ""));
                if (App.listCitizenProfile.First().AadhaarVerificationStatus != "Y")
                {
                    Application.Current.MainPage = new NavigationPage(new AadhaarPage());
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                }
            }
            else
            {
                Loading_activity.IsVisible = false;
                Entry_OTP.Text = null;
            }
            Loading_activity.IsVisible = false;
        }

        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new LanguagePage());
        }
    }
}

