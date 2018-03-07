using FreshMvvm;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Movil.Abstract;

namespace Xpinn.SportsGo.Movil.Controls
{
    public class InfiniteListView : ListView
    {
        IKeyboardHelper _keyboardHelper;
        TapGestureRecognizer _gestureDismiss;

        public static readonly BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(
                nameof(LoadMoreCommand),
                typeof(ICommand),
                typeof(InfiniteListView),
                default(ICommand),
                BindingMode.TwoWay,
                propertyChanged: (bindableObject, oldValue, newValue) =>
                {
                    InfiniteListView listView = (InfiniteListView)bindableObject;
                    listView.LoadMoreCommandPropertyChanged(listView, (ICommand)oldValue, (ICommand)newValue);
                }
            );

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        void LoadMoreCommandPropertyChanged(InfiniteListView bindable, ICommand oldValue, ICommand newValue)
        {
            if (newValue != default(ICommand))
            {
                ItemAppearing += InfiniteListView_ItemAppearing;
            }
            else
            {
                ItemAppearing -= InfiniteListView_ItemAppearing;
            }
        }

        public static readonly BindableProperty DismissKeyboardOnTouchProperty =
            BindableProperty.Create(
                nameof(DismissKeyboardOnTouch),
                typeof(bool),
                typeof(InfiniteListView),
                default(bool),
                BindingMode.TwoWay,
                propertyChanged: (bindableObject, oldValue, newValue) =>
                {
                    InfiniteListView listView = (InfiniteListView)bindableObject;
                    listView.DismissKeyboardOnTouchPropertyChanged(listView, (bool)oldValue, (bool)newValue);
                }
        );

        public bool DismissKeyboardOnTouch
        {
            get { return (bool)GetValue(DismissKeyboardOnTouchProperty); }
            set { SetValue(DismissKeyboardOnTouchProperty, value); }
        }

        void DismissKeyboardOnTouchPropertyChanged(InfiniteListView bindable, bool oldValue, bool newValue)
        {
            if (newValue != default(bool))
            {
                GestureRecognizers.Add(_gestureDismiss);
            }
            else
            {
                GestureRecognizers.Remove(_gestureDismiss);
            }
        }

        public InfiniteListView() : this(ListViewCachingStrategy.RetainElement)
        {
        }

        public InfiniteListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            _keyboardHelper = FreshIOC.Container.Resolve<IKeyboardHelper>();

            _gestureDismiss = new TapGestureRecognizer
            {
                Command = new Command(() => _keyboardHelper.HideKeyboard())
            };
        }

        void InfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = ItemsSource as IList;

            if (items != null && items.Count > 0 && e.Item == items[items.Count - 1])
            {
                if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                    LoadMoreCommand.Execute(null);
            }
        }
    }
}
