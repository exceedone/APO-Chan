using Apo_Chan.Items;
using Apo_Chan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apo_Chan.Managers
{
    /// <summary>
    /// "UserManager" is same name in  namespace "Android.OS".
    /// So rename User"s"Manager.
    /// </summary>
    public partial class ReportGroupManager : BaseManager<ReportGroupItem>
    {
        static ReportGroupManager()
        {
            defaultInstance = new ReportGroupManager();
        }
        public ReportGroupManager() : base() { }

        public static ReportGroupManager DefaultManager
        {
            get
            {
                return defaultInstance as ReportGroupManager;
            }
            private set
            {
                defaultInstance = value;
            }
        }
        
        public async Task<ReportGroupItem> GetItemAsync(Expression<Func<ReportGroupItem, bool>> expression)
        {
            // get from Azure Mobile Apps
            try
            {
                // not token update info
                //await BaseAuthProvider.RefreshProfile();
                IEnumerable<ReportGroupItem> items = await this.dataTable
                    .Where(expression)
                    .ToEnumerableAsync();

                if (!items.Any()) { return null; }
                return items.ToList().FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"-------------------[Debug] UsersManager Invalid sync operation: " + msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"-------------------[Debug] UsersManager Sync error: " + e.Message);
            }
            return null;
        }

    }
}
