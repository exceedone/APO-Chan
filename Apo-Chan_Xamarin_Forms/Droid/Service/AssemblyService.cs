using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Apo_Chan.Models;
using Xamarin.Forms;
    
[assembly: Dependency(typeof(AssemblyService))]
public class AssemblyService : IAssemblyService
{
    //アプリ名称を取得する
    public string GetPackageName()
    {
        Context context = Forms.Context;    //Android.App.Application.Context;
        var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).PackageName;
        return name;
    }

    //アプリバージョン文字列を取得する
    public string GetVersionName()
    {
        Context context = Forms.Context;    //Android.App.Application.Context;
        var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
        return name;
    }

    //アプリバージョンコードを取得する
    public int GetVersionCode()
    {
        Context context = Forms.Context;    //Android.App.Application.Context;
        var code = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
        return code;
    }
}