using Apo_Chan.Models;
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

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Report> ReportItems { get; set; }

        public UserReportListViewModel()
        {
            //var manager = new ReportManager();

            //try
            //{
            //    this.ReportItems = (ObservableCollection<Report>)ReportManager.DefaultManager.GetItemsAsync().Result;
            //}
            //catch
            //{

            //}
        }

        async public void SetItemsAsync()
        {
            this.ReportItems = await ReportManager.DefaultManager.GetItemsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
