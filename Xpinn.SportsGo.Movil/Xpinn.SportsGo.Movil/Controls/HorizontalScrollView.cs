using System;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace Xpinn.SportsGo.Movil.Controls
{
    public class HorizontalScrollView : ScrollView
    {
        readonly StackLayout _imageStack;

        public HorizontalScrollView()
        {
            Orientation = ScrollOrientation.Horizontal;

            _imageStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            Content = _imageStack;
        }

        public new IList<View> Children
        {
            get
            {
                return _imageStack.Children;
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IList),
                typeof(HorizontalScrollView),
                default(IList),
                BindingMode.TwoWay,
                propertyChanging: (bindableObject, oldValue, newValue) =>
                {
                    ((HorizontalScrollView)bindableObject).ItemsSourceChanging();
                },
                propertyChanged: (bindableObject, oldValue, newValue) =>
                {
                    ((HorizontalScrollView)bindableObject).ItemsSourceChanged(bindableObject, oldValue, newValue);
                }
            );

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {

                SetValue(ItemsSourceProperty, value);
            }
        }

        void ItemsSourceChanging()
        {
            if (ItemsSource == null)
                return;
        }

        void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (ItemsSource == null)
                return;

            IEnumerable collection = newValue as IEnumerable;
            if (collection != null)
            {
                CreateItems(collection);

                INotifyCollectionChanged notifyCollection = collection as INotifyCollectionChanged;
                if (notifyCollection != null)
                {
                    notifyCollection.CollectionChanged += (sender, args) =>
                    {
                        if (args.NewItems != null)
                        {
                            CreateItems(args.NewItems);
                        }
                        if (args.OldItems != null)
                        {
                            // not supported
                            _imageStack.Children.RemoveAt(args.OldStartingIndex);
                        }
                    };
                }
            }
        }

        void CreateItems(IEnumerable collection)
        {
            _imageStack.Children.Clear();

            foreach (var newItem in collection)
            {
                View view = (View)ItemTemplate.CreateContent();
                BindableObject bindableObject = view as BindableObject;
                if (bindableObject != null)
                    bindableObject.BindingContext = newItem;
                _imageStack.Children.Add(view);
            }
        }

        public DataTemplate ItemTemplate
        {
            get;
            set;
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(
                nameof(SelectedItem),
                typeof(object),
                typeof(HorizontalScrollView),
                null,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((HorizontalScrollView)bindable).UpdateSelectedIndex();
                }
            );

        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        void UpdateSelectedIndex()
        {
            if (SelectedItem == BindingContext)
                return;

            SelectedIndex = Children
                .Select(c => c.BindingContext)
                .ToList()
                .IndexOf(SelectedItem);

        }

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(
                nameof(SelectedIndex),
                typeof(int),
                typeof(HorizontalScrollView),
                0,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((HorizontalScrollView)bindable).UpdateSelectedItem();
                }
            );

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        void UpdateSelectedItem()
        {
            SelectedItem = SelectedIndex > -1 ? Children[SelectedIndex].BindingContext : null;
        }
    }
}
