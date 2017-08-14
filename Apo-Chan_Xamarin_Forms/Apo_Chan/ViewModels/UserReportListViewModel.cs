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

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Report> ReportItems { get; set; }

        public UserReportListViewModel()
        {
            //ReportItems = new ObservableCollection<Report>();
            ReportItems = (ObservableCollection<Report>)ReportManager.GetItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
