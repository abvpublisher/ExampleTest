using bj.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace bj.ViewModels
{
    //[QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel 
    {
        public ItemDetailViewModel(ItemsBjMessageData data)
        {
            this.Id = data.id;
            this.text = data.text;            
        }

        int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }


        private string text;
        private string description;        
         
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }        

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                //Id = item.Id;
                Text = item.Text;
                Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
