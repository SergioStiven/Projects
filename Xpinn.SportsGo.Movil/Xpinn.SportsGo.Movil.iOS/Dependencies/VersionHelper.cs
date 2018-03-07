using Foundation;
using Xpinn.SportsGo.Movil.Abstract;

namespace Xpinn.SportsGo.Movil.iOS.Dependencies
{
    [Preserve(AllMembers = true)]
    class VersionHelper : IVersionHelper
    {
        public string VersionName
        {
            get
            {
                NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
                return ver.ToString();
            }
        }

        public string VersionCode
        {
            get
            {
                NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];
                return ver.ToString();
            }
        }
    }
}