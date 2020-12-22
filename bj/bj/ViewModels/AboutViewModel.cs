using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace bj.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "Авторизация";

            SaveCommand = new Command(OnSave, ValidateSave);

            //ExitCommand = new Command(OnExit);

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            if (User.UserType == "Админ")
            {
                this.IsVisibleExitButton = true;
                this.IsVisibleLoginButton = false;
            }

            //MessagingCenter.Subscribe<string, bool>("UserLogin", "UserLogin", (obj, item) =>
            //{
            //    this.IsVisibleExitButton = true;
            //    this.IsVisibleLoginButton = false;
            //});
        }
        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(UserName)
                && !String.IsNullOrWhiteSpace(Password);
        }
        string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        string _developer = User.Developer;
        public string Developer
        {
            get { return _developer; }
            set
            {
                SetProperty(ref _developer, value);
                User.Developer = this.Developer;
            }
        }
        bool _isVisibleLoginButton = true;
        public bool IsVisibleLoginButton
        {
            get { return _isVisibleLoginButton; }
            set { SetProperty(ref _isVisibleLoginButton, value); }
        }
        bool _isVisibleExitButton = false;
        public bool IsVisibleExitButton
        {
            get { return _isVisibleExitButton; }
            set { SetProperty(ref _isVisibleExitButton, value); }
        }

        public Command SaveCommand { get; }

        private async void OnSave()
        {
            try
            {
                LoginStatus a = new LoginStatus();


                string uriStr = @"https://uxcandy.com/~shapoval/test-task-backend/v2/login?developer=" + User.Developer;

                var form = new Dictionary<string, string>
                {
                    {"username", this.UserName},
                    {"password", this.Password}
                };

                HttpClient client = new HttpClient();
                HttpResponseMessage tokenResponse = await client.PostAsync(uriStr, new FormUrlEncodedContent(form));
                var jsonContent = await tokenResponse.Content.ReadAsStringAsync();

                a = JsonConvert.DeserializeObject<LoginStatus>(jsonContent);

                if (a.Status == "ok")
                {
                    await Application.Current.MainPage.DisplayAlert("Обратная связь", "вход выполнен", "OK");

                    //Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);

                    //await SecureStorage.SetAsync("userToken", Guid.NewGuid().ToString());

                    //PropertiesRW.Write("userToken", Guid.NewGuid().ToString());
                    PropertiesRW.Write("userToken", a.Message.Token);

                    this.IsVisibleExitButton = true;
                    this.IsVisibleLoginButton = false;
                    User.UserType = "Админ";
                    User.Token = a.Message.Token;
                    MessagingCenter.Send("UserLogin", "UserLogin", true);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Обратная связь", "ошибка авторизации, неправильные реквизиты доступа", "OK");
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            // This will pop the current page off the navigation stack
            //await Shell.Current.GoToAsync("..");
        }
        public Command ExitCommand { get; }
        private void OnExit()
        {
            SecureStorage.Remove("userToken");
        }
    }
    public class LoginStatus
    {
        public LoginStatus()
        {
            Message = new LoginStatusMessage();
        }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public LoginStatusMessage Message { get; set; }
    }
    public class LoginStatusMessage
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}