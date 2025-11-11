using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace GACAppeal
{	
	public partial class AppealsTabbedPage : TabbedPage
	{	
		public AppealsTabbedPage (int indexer = 0)
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetIconColor(this, Color.FromHex("#198754"));
            }
            Children.Add(new TotalPage());
            Children.Add(new DraftPage());
            Children.Add(new UnderProcessPage());
            Children.Add(new NotAdmittedPage());
            Children.Add(new DisposedPage());
            CurrentPage = Children[indexer];
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
        }


        void Btn_Home_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new HomePage());
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
    }
}

