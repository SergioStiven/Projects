using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xpinn.SportsGo.Movil.Android.Renderers;
using BaseAndroid = Android;

[assembly: ExportRenderer(typeof(ViewCell), typeof(NoSelectorColorViewCellRenderer))]
namespace Xpinn.SportsGo.Movil.Android.Renderers
{
    [Preserve(true, true)]
    public class NoSelectorColorViewCellRenderer : ViewCellRenderer
    {
        protected override BaseAndroid.Views.View GetCellCore(Cell item, BaseAndroid.Views.View convertView, BaseAndroid.Views.ViewGroup parent, BaseAndroid.Content.Context context)
        {
            var cell = base.GetCellCore(item, convertView, parent, context);

            var listView = parent as BaseAndroid.Widget.ListView; 

            if (listView != null)
            {
                listView.SetSelector(BaseAndroid.Resource.Color.Transparent);
                listView.CacheColorHint = BaseAndroid.Graphics.Color.Transparent;
            }

            return cell;
        }
    }
}