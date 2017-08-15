using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Apo_Chan.Managers;
using Apo_Chan.Items;

using Prism.Commands;
using Prism.Mvvm;

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
