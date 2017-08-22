using Apo_Chan.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Apo_Chan.Test
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage : ContentPage
	{
        public ObservableCollection<ReportItem> ReportItems { get; set; }

        public Command GoBackCommand { get; set; }

        public bool HasBackButton { get; set; }

        public TestPage ()
		{
			InitializeComponent ();

            BindingContext = this;
            ReportItems = TestReportLocalStore.GetItems();
            reportList.ItemsSource = ReportItems;

            GoBackCommand = new Command(ExecuteGoBack);
            HasBackButton = true;
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnPreviousButtonClicked", "Not implemented!", "OK");
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("OnNextButtonClicked", "Not implemented!", "OK");
        }

        private void ExecuteGoBack()
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug] ExecuteGoBack 0");
        }
    }
}