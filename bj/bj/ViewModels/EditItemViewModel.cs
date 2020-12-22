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
    [QueryProperty(nameof(Id), nameof(Id))]
    //[QueryProperty(nameof(Text), nameof(Text))]    
    public class EditItemViewModel : BaseViewModel
    {
        public EditItemViewModel()
        {
            EditCommand = new Command(OnEdit, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => EditCommand.ChangeCanExecute();
        }
        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Text);
        }
        string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);

                if (_id != null)
                {
                    ItemsBjMessageData a = User.GetUserData(int.Parse(_id));
                    Text = a.text;
                    OldText = a.text;
                    IsTaskExecuted = a.Status;
                    IsEditAdmin = a.IsEditAdmin;
                }
            }
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        public string OldText { get; set; }

        bool _isTaskExecuted;
        public bool IsTaskExecuted
        {
            get { return _isTaskExecuted; }
            set { SetProperty(ref _isTaskExecuted, value); }
        }
        public bool IsEditAdmin { get; set; }

        public Command EditCommand { get; }
        public Command CancelCommand { get; }
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        private async void OnEdit()
        {
            string uriStr = @"https://uxcandy.com/~shapoval/test-task-backend/v2/edit/" + Id + "?developer=" + User.Developer;

            var form = new Dictionary<string, string>
            {
               {"token", User.Token},
               {"status", this.IsTaskExecuted? "10" : "0" },
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
                    if (Text != OldText)
                    {
                        User.SetUserDataIsEditAdmin(int.Parse(_id));
                    };

                    await Application.Current.MainPage.DisplayAlert("Обратная связь", "задача успешно отредактирована", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Обратная связь", "ошибка редактирования", "OK");
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
}