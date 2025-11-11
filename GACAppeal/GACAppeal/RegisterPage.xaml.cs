using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace GACAppeal
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
        }
        void Rdo_Email_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (Rdo_Email.IsChecked)
            {
                Entry_username.Placeholder = "Enter Email";
                Entry_username.Text = null;
                Entry_username.MaxLength = 100;
                Entry_username.Keyboard = Keyboard.Email;
            }

        }

        void Rdo_Mobile_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (Rdo_Mobile.IsChecked)
            {
                Entry_username.Placeholder = "Enter 10 Digit Mobile No";
                Entry_username.Text = null;
                Entry_username.MaxLength = 10;
                Entry_username.Keyboard = Keyboard.Telephone;
            }
        }
        void Btn_SendOTP_Clicked(System.Object sender, System.EventArgs e)
        {
        }

        void Btn_VerifyOTP_Clicked(System.Object sender, System.EventArgs e)
        {
        }
    }
}

