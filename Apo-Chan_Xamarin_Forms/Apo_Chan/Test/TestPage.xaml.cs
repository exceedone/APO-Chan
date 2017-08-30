using Apo_Chan.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Apo_Chan.Test
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<ReportItem> ReportItems { get; set; }

        public Command GoBackCommand { get; set; }

        public Command NextCommand { get; set; }

        public Command PreviousCommand { get; set; }

        public bool HasBackButton { get; set; }

        private bool isLoading = false;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public TestPage ()
        {
            InitializeComponent();

            BindingContext = this;
            GetItems();

            GoBackCommand = new Command(ExecuteGoBack);
            NextCommand = new Command(OnNextButtonClicked);
            PreviousCommand = new Command(OnPreviousButtonClicked);

            HasBackButton = true;
            IsLoading = true;

            reportList.ItemsSource = ReportItems;
        }

        private async Task GetItems()
        {
            ReportItems = await TestReportLocalStore.GetItems(2017,8);
        }

        private async void OnPreviousButtonClicked(/*object sender, EventArgs e*/)
        {
            await DisplayAlert("OnPreviousButtonClicked", "Not implemented!", "OK");
            //HasBackButton = !HasBackButton;
        }

        private async void OnNextButtonClicked(/*object sender, EventArgs e*/)
        {
            await DisplayAlert("OnNextButtonClicked", "Not implemented!", "OK");
            IsLoading = false;
        }

        private async void ExecuteGoBack()
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug] ExecuteGoBack 0");
            IsLoading = false;
            await DisplayAlert("ExecuteGoBack", "IsLoading: " + IsLoading, "OK");
        }
    }
}