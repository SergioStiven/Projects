using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Controls
{
    public class InvertedInfiniteListView : ListView
    {
        public static readonly BindableProperty LoadMoreCommandProperty =
                BindableProperty.Create(
                nameof(LoadMoreCommand),
                typeof(ICommand),
                typeof(InvertedInfiniteListView),
                default(ICommand),
                BindingMode.TwoWay
        );

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        bool _esCargaInicial = true;

        public InvertedInfiniteListView()
        {
            ItemAppearing += InvertedInfiniteListView_ItemAppearing;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var items = ItemsSource as IList;

            if (items != null && items.Count > 0)
            {
                ScrollTo(items[items.Count - 1], ScrollToPosition.MakeVisible, false);
            }
        }

        void InvertedInfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = ItemsSource as IList;

            if (items != null && items.Count > 0 && e.Item == items[0])
            {
                if (_esCargaInicial)
                {
                    ScrollTo(items[items.Count - 1], ScrollToPosition.MakeVisible, false);
                    _esCargaInicial = false;
                }
                else
                {
                    if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                        LoadMoreCommand.Execute(null);
                }
            }
        }

        void InvertedInfiniteListView_ItemSourceChanged(object sender, EventArgs e)
        {
            var items = ItemsSource as IList;

            if (items != null && items.Count > 0)
            {
                ScrollTo(items[items.Count - 1], ScrollToPosition.MakeVisible, false);
            }
        }

    }
}
