using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Apo_Chan.Items;
using System.Threading.Tasks;

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

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await addButton.ScaleTo(0.6, 50);
            await addButton.ScaleTo(1.8, 50);
            await Task.Delay(100);
            await addButton.ScaleTo(1, 50);
        }
    }
}