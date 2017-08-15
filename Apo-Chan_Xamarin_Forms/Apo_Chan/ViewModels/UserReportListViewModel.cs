using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : BindableBase
    {
        public UserReportListViewModel()
        {
            this.ReportItems = new ObservableCollection<ReportItem>();
        }

        private ObservableCollection<ReportItem> reportItems;
        public ObservableCollection<ReportItem> ReportItems
        {
            get
            {
                return reportItems;
            }
            set
            {
                SetProperty(ref this.reportItems, value);
            }
        }

        public async void SetItemsAsync()
        {
            this.ReportItems = await ReportManager.DefaultManager.GetItemsAsync();
        }
    }
}
