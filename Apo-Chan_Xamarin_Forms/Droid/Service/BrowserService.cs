using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.CustomTabs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Apo_Chan.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(AssemblyService))]
public class BrowserService
{
    /// <summary>
    /// OpenBrowser
    /// </summary>
    /// <param name="url"></param>
    public static void OpenUrl(Activity activity, string url)
    {
        var builder = new CustomTabsIntent.Builder()
            .SetToolbarColor(System.Drawing.Color.Black.ToArgb());
        var intent = builder.Build();
        var mgr = new CustomTabsActivityManager(activity);

        mgr.CustomTabsServiceConnected += delegate {
            mgr.LaunchUrl(url, intent);
        };
        mgr.BindService();
    }
}