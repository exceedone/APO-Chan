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

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnPreviousButtonClicked", "I'm doing nothing", "OK");
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnNextButtonClicked", "I'm doing nothing", "OK");
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