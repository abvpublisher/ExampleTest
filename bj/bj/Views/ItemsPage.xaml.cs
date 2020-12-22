using bj.Models;
using bj.ViewModels;
using bj.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace bj.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;
        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
        private async void sfDataGrid_GridDoubleTapped(object sender, Syncfusion.SfDataGrid.XForms.GridDoubleTappedEventArgs e)
        {
            if (string.IsNullOrEmpty(User.Token)) return;

            if (!(e.RowData is ItemsBjMessageData item)) return;

            //await Shell.Current.GoToAsync(nameof(NewItemPage)); &{nameof(EditItemViewModel.Text)}={item.text}&{nameof(EditItemViewModel.IsTaskExecuted)}={item.Status}

            await Shell.Current.GoToAsync($"{nameof(EditItemPage)}?{nameof(EditItemViewModel.Id)}={item.id}");
        }
    }
}