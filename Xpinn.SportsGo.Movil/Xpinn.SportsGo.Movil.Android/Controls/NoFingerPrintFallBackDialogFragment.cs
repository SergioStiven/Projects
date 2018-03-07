using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.Fingerprint.Dialog;
using Xamarin.Forms.Internals;

namespace Xpinn.SportsGo.Movil.Android.Controls
{
    [Preserve(true, true)]
    public class NoFingerPrintFallBackDialogFragment : FingerprintDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            view.FindViewById<Button>(Resource.Id.fingerprint_btnFallback).Visibility = ViewStates.Gone;

            return view;
        }
    }
}