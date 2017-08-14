using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apo_Chan
{
    public static class ReportManager
    {
        public static IList<Report> ReportStore { get; set; }

        static ReportManager()
        {
            ReportStore = new ObservableCollection<Report>
            {
                new Report()
                {
                    Id = "123", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,38,30),
                    ReportTitle = "report 1"
                },
                new Report()
                {
                    Id = "124", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,40,0),
                    ReportTitle = "report 2"
                },
                new Report()
                {
                    Id = "125", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(19,45,30),
                    ReportTitle = "report 3"
                },
                new Report()
                {
                    Id = "126", RefUserId = "asd",
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(20,30,0),
                    ReportTitle = "report 4"
                },
            };
        }

        public static IList<Report> GetItems()
        {
            return ReportStore;
        }
    }
}
