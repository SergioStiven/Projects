using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xpinn.SportsGo.Movil.iOS.Renderers;

[assembly: ExportRenderer(typeof(ViewCell), typeof(NoSelectorColorViewCellRenderer))]
namespace Xpinn.SportsGo.Movil.iOS.Renderers
{
    // DISABLES FLOWLISTVIEW ROW HIGHLIGHT
    [Foundation.Preserve(AllMembers = true)]
    public class NoSelectorColorViewCellRenderer : ViewCellRenderer
    {
        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);

            if (cell != null)
            {
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            }

            return cell;
        }
    }
}