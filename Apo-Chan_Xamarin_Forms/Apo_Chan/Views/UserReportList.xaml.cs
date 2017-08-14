using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Apo_Chan.Models;
using Apo_Chan.ViewModels;

namespace Apo_Chan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserReportList : ContentPage
    {
        public UserReportListViewModel viewModel;

        public UserReportList()
        {
            InitializeComponent();

            BindingContext = viewModel = new UserReportListViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnPreviousButtonClicked", "I'm doing nothing", "OK");
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnNextButtonClicked", "I'm doing nothing", "OK");
            //viewModel.ReportItems.Add(new Report()
            //{
            //    Id = "123",
            //    RefUserId = "asd",
            //    ReportStartDate = new DateTime(2017, 8, 10),
            //    ReportStartTime = new TimeSpan(18, 38, 30),
            //    ReportTitle = "report 1"
            //});
        }

        private async void OnReportListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped: " + (((ListView)sender).SelectedItem as Report).ReportTitle, "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}