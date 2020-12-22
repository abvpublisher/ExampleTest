using bj.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace bj.Views
{
    public partial class AboutPage : ContentPage
    {
        private readonly AboutViewModel viewModel;
        public AboutPage()
        {
            BindingContext = viewModel = new AboutViewModel();

            InitializeComponent();
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            //SecureStorage.Remove("userToken");
            PropertiesRW.Write("userToken", string.Empty);            

            User.UserType = "Пользователь";
            User.Token = string.Empty;

            this.viewModel.IsVisibleExitButton = false;
            this.viewModel.IsVisibleLoginButton = true;

            MessagingCenter.Send("UserLogin", "UserLogin", false);
        }
    }
}