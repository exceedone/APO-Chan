using Foundation;
using Xamarin.Forms;
using Apo_Chan.Models;

[assembly: Dependency(typeof(AssemblyService))]
public class AssemblyService : IAssemblyService
{
    //アプリ名称を取得する
    public string GetPackageName()
    {
        string name = NSBundle.MainBundle.InfoDictionary["CFBundleDisplayName"].ToString();
        return name.ToString();
    }
    //アプリバージョン文字列を取得する
    public string GetVersionName()
    {
        string name = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
        return name.ToString();
    }
    //アプリバージョンコードを取得する
    public int GetVersionCode()
    {
        var code = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
        return int.Parse(code);
    }
}