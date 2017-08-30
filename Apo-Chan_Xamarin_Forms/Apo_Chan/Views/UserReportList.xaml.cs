using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Apo_Chan.Items;

namespace Apo_Chan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserReportList : ContentPage
    {
        public UserReportList()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            reportList.RefreshCommand.Execute(null);
        }
    }
}