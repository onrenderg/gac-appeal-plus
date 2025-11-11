using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GACAppeal
{	
	public partial class LanguagePage : ContentPage
	{
        Models.LanguageMasterDatabase languageMasterDatabase = new Models.LanguageMasterDatabase();
        List<Models.LanguageMaster> List_LanguageMaster;

        public LanguagePage ()
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                Loading_activity.IsVisible = true;
                var service = new Models.HitService();
                await service.RefreshMasters_Get();
                languageMasterDatabase = new Models.LanguageMasterDatabase();
                Picker_Language.ItemsSource = List_LanguageMaster = languageMasterDatabase.FillLanguageMaster();
                Picker_Language.ItemDisplayBinding = new Binding("languageDescriptionlocal");
                Picker_Language.SelectedIndex = List_LanguageMaster.FindIndex(x => x.LanguageCode == Preferences.Get("LanguageCode", "en-IN"));
                Loading_activity.IsVisible = false;
                App.LableValue(Lbl_Nav_title, "GrievanceAppellateCommittee");
            });
            App.LableValue(Lbl_Nav_title, "GrievanceAppellateCommittee");
            languageMasterDatabase = new Models.LanguageMasterDatabase();
            Picker_Language.ItemsSource = List_LanguageMaster =  languageMasterDatabase.FillLanguageMaster();
            Picker_Language.ItemDisplayBinding = new Binding("languageDescriptionlocal");
            Picker_Language.SelectedIndex = List_LanguageMaster.FindIndex(x => x.LanguageCode == Preferences.Get("LanguageCode", "en-IN"));
		}

        void Btn_Update_Clicked(System.Object sender, System.EventArgs e)
        {

        }

        void Picker_Language_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (Picker_Language.SelectedIndex != -1)
            {
                if (Preferences.Get("LanguageCode", "en-IN") != List_LanguageMaster.ElementAt(Picker_Language.SelectedIndex).LanguageCode)
                {
                    Preferences.Set("LanguageCode", List_LanguageMaster.ElementAt(Picker_Language.SelectedIndex).LanguageCode);
                    Navigation.PopAsync();
                }
            }
        }
    }
}

