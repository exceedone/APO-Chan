using Apo_Chan.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apo_Chan.Managers
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
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,38,30),
                    ReportTitle = "Meeting with company ABC"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,10), ReportStartTime = new TimeSpan(18,40,0),
                    ReportTitle = "Meeting with company DEF"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(19,45,30),
                    ReportTitle = "Develop at Home"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,11), ReportStartTime = new TimeSpan(20,30,0),
                    ReportTitle = "Cafe Develop"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,15), ReportStartTime = new TimeSpan(09,00,0),
                    ReportTitle = "Writing technical blog"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,16), ReportStartTime = new TimeSpan(14,00,0),
                    ReportTitle = "Push GitHub"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,19), ReportStartTime = new TimeSpan(16,30,0),
                    ReportTitle = "Discuss next features"
                },
                new ReportItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    RefUserId = userId,
                    ReportStartDate = new DateTime(2017,8,21), ReportStartTime = new TimeSpan(11,30,0),
                    ReportTitle = "Tracking project progress"
                },
            };
        }

        public static string GetUserId()
        {
            return userId;
        }

        public static ObservableCollection<ReportItem> GetItems()
        {
            return reportDB;
        }

        public static void InsertItem(ReportItem item)
        {
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
            }
        }

        public static bool UpdateItem(ReportItem item)
        {
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

        public static bool DeleteItem(ReportItem item)
        {
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

        public static ReportItem GetItem(string Id)
        {
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
