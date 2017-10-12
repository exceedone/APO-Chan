using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Apo_Chan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupList : ContentPage
    {
        public GroupList()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            groupList.RefreshCommand.Execute(null);
        }
    }
}