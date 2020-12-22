using bj.ViewModels;
using bj.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace bj
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(EditItemPage), typeof(EditItemPage));
        }

    }
}
