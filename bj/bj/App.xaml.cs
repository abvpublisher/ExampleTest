using bj.Services;
using bj.ViewModels;
using bj.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace bj
{
    public partial class App : Application
    {

        public App()
        {
            //Xamarin - Trial License Valid Until January 10, 2021
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY0OTU0QDMxMzgyZTMzMmUzMFQ2YjBKb0lieXBQdUsrdjFDVHVKWG5ka3JxMzFTUldwTjIzZWJEd2Y0WVk9");

            InitializeComponent();
            CheckPermission();
            DependencyService.Register<MockDataStore>();

            try
            {
                string userData = PropertiesRW.Ride("userData");
                if (!string.IsNullOrWhiteSpace(userData))
                {
                    User.InitUserData(userData);
                }                
                string userToken = string.Empty;
                userToken = PropertiesRW.Ride("userToken");
                if (!string.IsNullOrWhiteSpace(userToken))
                {                    
                    if (userToken != Guid.Empty.ToString())
                    {
                        bj.ViewModels.User.UserType = "Админ";
                        bj.ViewModels.User.Token = userToken;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            MainPage = new AppShell();
        }

        async private void CheckPermission()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await Current.MainPage.DisplayAlert("Need location", "Gunna need that location", "OK");
                    }

                    status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
                }

                if (status == PermissionStatus.Granted)
                {
                    //Query permission
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Current.Quit();                    
                }
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }        
    }
}
