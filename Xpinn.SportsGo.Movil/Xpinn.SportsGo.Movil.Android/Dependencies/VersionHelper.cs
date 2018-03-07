using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xpinn.SportsGo.Movil.Abstract;

namespace Xpinn.SportsGo.Movil.Android.Dependencies
{
    [Preserve(true, true)]
    class VersionHelper : IVersionHelper
    {
        public string VersionName
        {
            get
            {
                var context = MainApplication.CurrentContext;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
        }

        public string VersionCode
        {
            get
            {
                var context = MainApplication.CurrentContext;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode.ToString();
            }
        }
    }
}