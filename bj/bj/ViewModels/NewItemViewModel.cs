using bj.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace bj.ViewModels
{    
    public class NewItemViewModel : BaseViewModel
    {
        public NewItemViewModel()
        {             
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(UserName)
                && !String.IsNullOrWhiteSpace(Email)
                && !String.IsNullOrWhiteSpace(Text);
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }        

        string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                SetProperty(ref _email, value);
                if (!string.IsNullOrWhiteSpace(_email))
                {
                    if (Validator.EmailIsValid(this.Email))
                    {
                        EmailErr = string.Empty;
                    }
                    else
                    {
                        EmailErr = "email не валиден";
                    }
                }            
            }
        }
        string _emailErr;
        public string EmailErr
        {
            get { return _emailErr; }
            set { SetProperty(ref _emailErr, value); }
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        public Command EditCommand { get; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            if (Validator.EmailIsValid(this.Email))
            {
                EmailErr = string.Empty;
            }
            else
            {
                EmailErr = "email не валиден";
                return;
            }            

            string uriStr = @"https://uxcandy.com/~shapoval/test-task-backend/v2/create?developer=" + User.Developer;

            var form = new Dictionary<string, string>
                {
                    {"username", this.UserName},
                    {"email", this.Email},
                    {"text", this.Text}
                };

            try
            {
                ItemAddBj a = new ItemAddBj();

                HttpClient client = new HttpClient();
                HttpResponseMessage tokenResponse = await client.PostAsync(uriStr, new FormUrlEncodedContent(form));
                var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
                
                a = JsonConvert.DeserializeObject<ItemAddBj>(jsonContent);

                if (a.Status == "ok")
                {
                    await Application.Current.MainPage.DisplayAlert("Обратная связь", "задача успешно добавлена", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Обратная связь", "ошибка добавления", "OK");
                }                
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }            

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }        
    }

    public class ItemAddBj
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
