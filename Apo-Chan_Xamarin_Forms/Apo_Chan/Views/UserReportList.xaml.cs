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

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnPreviousButtonClicked", "Not implemented!", "OK");
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnNextButtonClicked", "Not implemented!", "OK");
        }

        private async void OnReportListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Detail Report", "Not implemented!", "OK");
            //await Navigation.PushAsync(new DetailReport(((ListView)sender).SelectedItem as ReportItem));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}