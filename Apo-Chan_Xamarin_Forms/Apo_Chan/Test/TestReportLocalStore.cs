using Apo_Chan.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apo_Chan.Test
{
    public static class TestReportLocalStore
    {
        private static string userId = Guid.NewGuid().ToString();
        private static ObservableCollection<ReportItem> reportDB { get; set; }

        static TestReportLocalStore()
        {
            reportDB = new ObservableCollection<ReportItem>
            {
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,7,10), ReportStartTime = new TimeSpan(8,30,00),
                    ReportEndDate = new DateTime(2017,7,10), ReportEndTime = new TimeSpan(9,00,00),
                    ReportTitle = "7 Meeting with company ABC"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(8,40,0),
                    ReportEndDate = new DateTime(2017,8,10), ReportEndTime = new TimeSpan(9,40,00),
                    ReportTitle = "8 Meeting with company DEF"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(19,45,00),
                    ReportEndDate = new DateTime(2017,8,11), ReportEndTime = new TimeSpan(22,00,00),
                    ReportTitle = "8 Develop at Home"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,12), ReportStartTime = new TimeSpan(20,30,0),
                    ReportEndDate = new DateTime(2017,8,12), ReportEndTime = new TimeSpan(21,15,00),
                    ReportTitle = "8 Cafe Develop"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,15), ReportStartTime = new TimeSpan(09,00,0),
                    ReportEndDate = new DateTime(2017,8,15), ReportEndTime = new TimeSpan(11,30,00),
                    ReportTitle = "8 Writing technical blog"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,16), ReportStartTime = new TimeSpan(14,00,0),
                    ReportEndDate = new DateTime(2017,8,16), ReportEndTime = new TimeSpan(14,15,00),
                    ReportTitle = "8 Push GitHub"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,9,22), ReportStartTime = new TimeSpan(14,0,0),
                    ReportEndDate = new DateTime(2017,9,22), ReportEndTime = new TimeSpan(15,0,0),
                    ReportTitle = "9 Test report #7"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,9,22), ReportStartTime = new TimeSpan(14,00,0),
                    ReportEndDate = new DateTime(2017,9,22), ReportEndTime = new TimeSpan(15,0,0),
                    ReportTitle = "9 Test report #8"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,9,22), ReportStartTime = new TimeSpan(14,00,0),
                    ReportEndDate = new DateTime(2017,9,22), ReportEndTime = new TimeSpan(15,0,0),
                    ReportTitle = "9 Test report #9"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,10,22), ReportStartTime = new TimeSpan(14,00,0),
                    ReportEndDate = new DateTime(2017,10,22), ReportEndTime = new TimeSpan(15,0,0),
                    ReportTitle = "10 Test report #10"
                },
            };
        }

        public static string GetUserId()
        {
            return userId;
        }

        public static async Task<ObservableCollection<ReportItem>> GetItems(int year, int month)
        {
            await Task.Delay(1600);
            ObservableCollection<ReportItem> reportMonth = new ObservableCollection<ReportItem>();
            foreach (var item in reportDB)
            {
                if (item.ReportStartDate.Year == year && item.ReportStartDate.Month == month)
                {
                    reportMonth.Add(item);
                }
            }

            return reportMonth;
        }

        public static async Task<bool> InsertItem(ReportItem item)
        {
            await Task.Delay(400);
            bool duplicated = false;
            if (item.Id == null)
            {
                item.Id = Guid.NewGuid().ToString();
            }
            else
            {
                foreach (var report in reportDB)
                {
                    if (report.Id.CompareTo(item.Id) == 0)
                    {
                        duplicated = true;
                    }
                }
            }
            if (!duplicated)
            {
                reportDB.Add(item);
                return true;
            }
            else
            {
                //no insert
                return false;
            }
        }

        public static async Task<bool> UpdateItem(ReportItem item)
        {
            await Task.Delay(400);
            if (item.Id == null)
            {
                return false;
            }
            else
            {
                foreach (var report in reportDB)
                {
                    if (report.Id.CompareTo(item.Id) == 0)
                    {
                        reportDB.Remove(report);
                        reportDB.Add(item);
                        return true;
                    }
                }
            }

            //no update
            return false;
        }

        public static async Task<bool> DeleteItem(ReportItem item)
        {
            await Task.Delay(400);
            if (item.Id == null)
            {
                return false;
            }
            else
            {
                foreach (var report in reportDB)
                {
                    if (report.Id.CompareTo(item.Id) == 0)
                    {
                        reportDB.Remove(report);
                        return true;
                    }
                }
            }

            //no delete
            return false;
        }

        public static async Task<ReportItem> GetItem(string Id)
        {
            await Task.Delay(200);
            foreach (var report in reportDB)
            {
                if (report.Id.CompareTo(Id) == 0)
                {
                    return report;
                }
            }

            //no report found
            return null;
        }
    }
}
