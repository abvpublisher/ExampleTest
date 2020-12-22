using bj.Models;
using bj.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace bj.ViewModels
{
    public static class User
    {
        static User()
        {
            Developer = "Boguk";
            UserType = "Пользователь";
            UserData = new ObservableCollection<ItemsBjMessageData>();
        }
        public static string Developer { get; set; }
        public static string UserType { get; set; }
        public static string Token { get; set; }
        public static ObservableCollection<ItemsBjMessageData> UserData { get; set; }

        public static void InitUserData(string ud)
        {
            ItemsBj a = new ItemsBj();

            a = JsonConvert.DeserializeObject<ItemsBj>(ud);

            UserData = new ObservableCollection<ItemsBjMessageData>(a.message.tasks);
        }
        public static ObservableCollection<ItemsBjMessageData> SetUserData(ObservableCollection<ItemsBjMessageData> ud)
        {
            foreach (ItemsBjMessageData item in ud)
            {
                foreach (ItemsBjMessageData userData in UserData)
                {
                    if (item.id == userData.id) item.IsEditAdmin = userData.IsEditAdmin;
                }
            }
            UserData = new ObservableCollection<ItemsBjMessageData>(ud);

            return UserData;
        }
        public static ItemsBjMessageData GetUserData(int id)
        {
            foreach (ItemsBjMessageData item in UserData)
            {
                if (item.id == id) return item;
            }
            return new ItemsBjMessageData();
        }

        public static void SetUserDataIsEditAdmin(int id)
        {
            foreach (ItemsBjMessageData item in UserData)
            {
                if (item.id == id) item.IsEditAdmin = true;
            }
        }
    }
    public static class Validator
    {

        static Regex ValidEmailRegex = CreateValidEmailRegex();

        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        internal static bool EmailIsValid(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);

            return isValid;
        }
    }
    public static class PropertiesRW
    {
        public static void Write(string key, string value)
        {
            Application.Current.Properties[key] = value;
            Application.Current.SavePropertiesAsync();
        }

        public static string Ride(string key)
        {
            string value = string.Empty;

            if (Application.Current.Properties.ContainsKey(key))
            {
                value = Application.Current.Properties[key].ToString();
            }

            return value;
        }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
