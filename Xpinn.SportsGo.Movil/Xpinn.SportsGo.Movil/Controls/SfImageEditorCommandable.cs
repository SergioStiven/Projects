using Syncfusion.SfImageEditor.XForms;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Controls
{
    public class SfImageEditorCommandable : SfImageEditor
    {
        //public static readonly BindableProperty StreamSourceProperty =
        //    BindableProperty.Create(
        //        nameof(StreamSource),
        //        typeof(Stream),
        //        typeof(SfImageEditorCommandable),
        //        default(Stream),
        //        BindingMode.OneWayToSource
        //    );

        //public Stream StreamSource
        //{
        //    get
        //    {                
        //        return GetValue(StreamSourceProperty) as Stream;
        //    }
        //    set { SetValue(StreamSourceProperty, value); }
        //}

        public static readonly BindableProperty ImageSavedCommandProperty =
            BindableProperty.Create(
                nameof(ImageSavedCommand),
                typeof(ICommand),
                typeof(SfImageEditorCommandable),
                default(ICommand),
                BindingMode.TwoWay,
                propertyChanged: (bindableObject, oldValue, newValue) =>
                {
                    SfImageEditorCommandable imageEditor = (SfImageEditorCommandable)bindableObject;
                    imageEditor.ImageSelectedChanged(imageEditor, (ICommand)oldValue, (ICommand)newValue);
                }
        );

        public ICommand ImageSavedCommand
        {
            get { return (ICommand)GetValue(ImageSavedCommandProperty); }
            set { SetValue(ImageSavedCommandProperty, value); }
        }

        void ImageSelectedChanged(SfImageEditorCommandable bindable, ICommand oldValue, ICommand newValue)
        {
            if (newValue != default(ICommand))
            {
                ImageSaved += SfImageEditorCommandable_ImageSaved;
            }
            else
            {
                ImageSaved -= SfImageEditorCommandable_ImageSaved;
            }
        }

        void SfImageEditorCommandable_ImageSaved(object sender, ImageSavedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.Location))
            {
                if (ImageSavedCommand != null && ImageSavedCommand.CanExecute(null))
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        ImageSavedCommand.Execute(GetStream());
                    }
                    else
                    {
                        ImageSavedCommand.Execute(args.Location);
                    }
                }
            }
        }
    }
}
