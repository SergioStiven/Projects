using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Controls
{
    class BindableToolbarItem : ToolbarItem
    {
        public BindableToolbarItem()
        {
            InitVisibility();
        }

        public BindableToolbarItem(string name, string icon, Action activated, ToolbarItemOrder order = ToolbarItemOrder.Default, int priority = 0) 
            : base(name, icon, activated, order, priority)
        {

        }

        private void InitVisibility()
        {
            OnIsVisibleChanged(this, false, IsVisible);
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static readonly BindableProperty IsVisibleProperty =
        BindableProperty.Create(
            nameof(IsVisible),
            typeof(bool),
            typeof(BindableToolbarItem),
            true,
            BindingMode.TwoWay,
            propertyChanged: OnIsVisibleChanged
        );

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            bool oldValueBool = (bool)oldvalue;
            bool newValueBool = (bool)newvalue;
            var item = bindable as BindableToolbarItem;

            if (item == null || item.Parent == null)
                return;

            var contentPage = item.Parent as ContentPage;

            if (contentPage == null)
                return;

            var items = contentPage.ToolbarItems;

            if (newValueBool && !items.Contains(item))
            {
                Device.BeginInvokeOnMainThread(() => { items.Add(item); });
            }
            else if (!newValueBool && items.Contains(item))
            {
                Device.BeginInvokeOnMainThread(() => { items.Remove(item); });
            }
        }
    }
}
