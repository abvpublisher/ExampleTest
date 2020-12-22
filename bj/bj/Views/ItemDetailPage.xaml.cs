using bj.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace bj.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage(ItemsBjMessageData data)
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel(data);
        }
    }
}