using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Movil.Abstract;

namespace Xpinn.SportsGo.Movil.MarkupExtension
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        readonly CultureInfo ci;
        static readonly Lazy<ResourceManager> _resourceManager = new Lazy<ResourceManager>(() => new ResourceManager(App.ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly));

        public TranslateExtension()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                if (App.ActualCulture != null)
                {
                    ci = App.ActualCulture;
                }
                else
                {
                    ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                }
            }
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            string translation = _resourceManager.Value.GetString(Text, ci);
            
            if (translation == null)
            {
                #if DEBUG
                throw new ArgumentException(
                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, App.ResourceId, ci.Name),
                    "Text");
                #else
                translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
                #endif
            }
            return translation;
        }
    }
}