using bj.Models;
using bj.Views;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace bj.ViewModels
{
    
    public class ItemsViewModel : BaseViewModel 
    {
        public ItemsViewModel() 
        {
            Title = "Задачи";
            this.UserType = User.UserType;

            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand().ConfigureAwait(true));

            ItemTapped = new Command<Item>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            SortItems = new ObservableCollection<SortDataItem>
            {
                new SortDataItem() { Id = EnumSortMode.Null, Name = "По умолчанию", ParamName = "id" },
                new SortDataItem() { Id = EnumSortMode.UserName, Name = "Имя пользователя", ParamName = "username" },
                new SortDataItem() { Id = EnumSortMode.Email, Name = "Email", ParamName = "email" },
                new SortDataItem() { Id = EnumSortMode.TaskStatus, Name = "Статус", ParamName = "status" }
            };
            this.IdSort = new SortDataItem() { Id = EnumSortMode.Null, Name = "По умолчанию", ParamName = "id" };
            //
            SortDirectionItems = new ObservableCollection<SortDirectionDataItem>
            {
                //new SortDirectionDataItem() { Id = EnumSortDirectionMode.Null, Name = "По умолчанию", ParamName = "id" },
                new SortDirectionDataItem() { Id = EnumSortDirectionMode.Asc, Name = "По возрастанию", ParamName = "asc" },
                new SortDirectionDataItem() { Id = EnumSortDirectionMode.Desc, Name = "По убыванию", ParamName = "desc" }
            };
            this.IdSortDirection = new SortDirectionDataItem() { Id = EnumSortDirectionMode.Asc, Name = "По возрастанию", ParamName = "asc" };
            //
            PageItems = new ObservableCollection<PageDataItem>
            {
                new PageDataItem() { Id = 1, Name = "Страница 1 из 1" }
            };
            this.IdPage = new PageDataItem() { Id = 1, Name = "Страница 1 из 1" };

            SetPageItemCommand = new Command(async () => await ExecuteSetPageItemCommand());
            //
            ItemsSourses = new ObservableCollection<ItemsBjMessageData>();
            //
            MessagingCenter.Subscribe<string, bool>("UserLogin", "UserLogin", (obj, item) =>
            {
                this.UserType = User.UserType;
            });
        }


        private Item _selectedItem;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }



        //Bj

        private string _userType;
        public string UserType
        {
            set { SetProperty(ref _userType, value); }
            get { return _userType; }
        }

        private ObservableCollection<ItemsBjMessageData> _itemsSourses;
        public ObservableCollection<ItemsBjMessageData> ItemsSourses
        {
            set { SetProperty(ref _itemsSourses, value); }
            get { return _itemsSourses; }
        }

        private int _totalTaskCount;
        public int TotalTaskCount
        {
            set
            {
                SetProperty(ref _totalTaskCount, value);
                SetPageItemCommand.Execute(null);
            }
            get { return _totalTaskCount; }
        }

        private int _totalPagesCount;
        public int TotalPagesCount
        {
            set
            {
                SetProperty(ref _totalPagesCount, value);
            }
            get { return _totalPagesCount; }
        }

        

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {                
                ItemsBj a = new ItemsBj();

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage();                

                try
                {
                    string _sort = "id";
                    string _sortDirection = "desc";
                    string _page = "1";

                    if (this.IdSort != null)
                    {
                        _sort = this.IdSort.ParamName;
                    }

                    if (this.IdSortDirection != null)
                    {
                        _sortDirection = this.IdSortDirection.ParamName;
                    }

                    if (this.IdPage != null)
                    {
                        _page = this.IdPage.Id.ToString();
                    }

                    string uriStr = "https://uxcandy.com/~shapoval/test-task-backend/v2/?developer=" + User.Developer + "&sort_field=" + _sort + "&sort_direction=" + _sortDirection + "&page=" + _page;

                    request.RequestUri = new Uri(uriStr);
                    request.Method = HttpMethod.Get;
                    request.Headers.Add("Accept", "application/json");

                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent responseContent = response.Content;
                        var json = await responseContent.ReadAsStringAsync();

                        a = JsonConvert.DeserializeObject<ItemsBj>(json);

                        //ItemsSourses = new ObservableCollection<ItemsBjMessageData>(a.message.tasks);
                        if (TotalTaskCount != a.message.total_task_count) TotalTaskCount = a.message.total_task_count;                        

                        ItemsSourses = User.SetUserData(a.message.tasks);

                        a.message.tasks = ItemsSourses;

                        string ns = JsonConvert.SerializeObject(a);
                        PropertiesRW.Write("userData", ns);
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Command SetPageItemCommand { get; set; }
        private async Task ExecuteSetPageItemCommand()
        {            
            await Task.Run(() => this.ApplySetPageItem());
        }
        public void ApplySetPageItem()
        {
            if (this.TotalTaskCount >= 3)
            {
                PageItems = new ObservableCollection<PageDataItem>
                {
                    new PageDataItem() { Id = 1, Name = "Страница 1 из 2" },
                    new PageDataItem() { Id = 2, Name = "Страница 2 из 2" }
                };
                this.TotalPagesCount = 2;
            }
            else
            {
                PageItems = new ObservableCollection<PageDataItem>
                {
                    new PageDataItem() { Id = 1, Name = "Страница 1 из 1" }                    
                };
                this.TotalPagesCount = 1;
            }
        }


        ObservableCollection<PageDataItem> _pageItems;
        public ObservableCollection<PageDataItem> PageItems
        {
            get { return _pageItems; }
            set { SetProperty(ref _pageItems, value); }
        }

        PageDataItem _idPage;
        public PageDataItem IdPage
        {
            get { return _idPage; }
            set
            {
                SetProperty(ref _idPage, value);
                this.LoadItemsCommand.Execute(null);
            }
        }

        ObservableCollection<SortDataItem> _sortItems;
        public ObservableCollection<SortDataItem> SortItems
        {
            get { return _sortItems; }
            set { SetProperty(ref _sortItems, value); }
        }
        SortDataItem _idSort;
        public SortDataItem IdSort
        {
            get { return _idSort; }
            set
            {
                if (value == null)
                {
                    SetProperty(ref _idSort, null);
                }
                else
                {
                    if (value.Id == EnumSortMode.Null)
                    {
                        SetProperty(ref _idSort, null);
                    }
                    else
                    {
                        SetProperty(ref _idSort, value);
                    }
                }
                this.IdSortDirection = new SortDirectionDataItem() { Id = EnumSortDirectionMode.Asc, Name = "По возрастанию", ParamName = "asc" };
                this.LoadItemsCommand.Execute(null);
            }
        }

        ObservableCollection<SortDirectionDataItem> _sortDirectionItems;
        public ObservableCollection<SortDirectionDataItem> SortDirectionItems
        {
            get { return _sortDirectionItems; }
            set { SetProperty(ref _sortDirectionItems, value); }
        }
        SortDirectionDataItem _idSortDirection;
        public SortDirectionDataItem IdSortDirection
        {
            get { return _idSortDirection; }
            set
            {
                if (value == null)
                {
                    SetProperty(ref _idSortDirection, null);
                }
                else
                {
                    if (value.Id == EnumSortDirectionMode.Null)
                    {
                        SetProperty(ref _idSortDirection, null);
                    }
                    else
                    {
                        SetProperty(ref _idSortDirection, value);
                    }
                }
                this.LoadItemsCommand.Execute(null);
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}"); 

        }
    }

    public class ItemsBj : BaseViewModel
    {
        public ItemsBj()
        {
            message = new ItemsBjMessage();
        }
        string _status;
        public string status
        { 
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        ItemsBjMessage _message;
        public ItemsBjMessage message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
    }
    public class ItemsBjMessage : BaseViewModel
    {
        public ItemsBjMessage()
        {
            tasks = new ObservableCollection<ItemsBjMessageData>();
        }

        int _total_task_count;
        public int total_task_count
        {
            get { return _total_task_count; }
            set { SetProperty(ref _total_task_count, value); }
        }
        public ObservableCollection<ItemsBjMessageData> tasks { get; set; }
    }
    public class ItemsBjMessageData : BaseViewModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string text { get; set; }

        [JsonProperty("status")]
        bool _status;
        public bool Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        public string image_path { get; set; }
        public bool IsEditAdmin { get; set; }
    }
    public enum EnumTaskStatus
    {
        Null = -1,
        NoOk = 0,
        Ok = 10
    }


    public class SortDataItem
    {
        public EnumSortMode Id { get; set; }
        public string Name { get; set; }
        public string ParamName { get; set; }
    }
    public enum EnumSortMode
    {
        Null = -1,
        UserName = 1,
        Email = 2,
        TaskStatus = -1
    }

    public class SortDirectionDataItem
    {
        public EnumSortDirectionMode Id { get; set; }
        public string Name { get; set; }
        public string ParamName { get; set; }
    }
    public enum EnumSortDirectionMode
    {
        Null = -1,
        Asc = 1,
        Desc = 2
    }

    public class PageDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}