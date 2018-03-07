using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoticiasViewCell : ViewCell
    {
        public NoticiasViewCell()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty IrPersonaProperty =
            BindableProperty.Create(
            nameof(IrPersona),
            typeof(ICommand),
            typeof(NoticiasViewCell),
            default(ICommand),
            BindingMode.TwoWay
        );

        public ICommand IrPersona
        {
            get { return (ICommand)GetValue(IrPersonaProperty); }
            set { SetValue(IrPersonaProperty, value); }
        }

        public static readonly BindableProperty InteracturarIconoPublicacionProperty =
            BindableProperty.Create(
            nameof(InteracturarIconoPublicacion),
            typeof(ICommand),
            typeof(NoticiasViewCell),
            default(ICommand),
            BindingMode.TwoWay
        );

        public ICommand InteracturarIconoPublicacion
        {
            get { return (ICommand)GetValue(InteracturarIconoPublicacionProperty); }
            set { SetValue(InteracturarIconoPublicacionProperty, value); }
        }

        public static readonly BindableProperty InteracturarPublicacionProperty =
            BindableProperty.Create(
            nameof(InteracturarPublicacion),
            typeof(ICommand),
            typeof(NoticiasViewCell),
            default(ICommand),
            BindingMode.TwoWay
        );

        public ICommand InteracturarPublicacion
        {
            get { return (ICommand)GetValue(InteracturarPublicacionProperty); }
            set { SetValue(InteracturarPublicacionProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            TimeLineNoticias item = BindingContext as TimeLineNoticias;

            if (item == null) return;

            UrlImagenPerfil.Source = null;
            if (!string.IsNullOrWhiteSpace(item.UrlImagenPerfil))
            {
                UrlImagenPerfil.Source = item.UrlImagenPerfil;
            }
            else
            {
                UrlImagenPerfil.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
            }

            UrlArchivoPublicacion.Source = null;
            if (item.EsVideo)
            {
                UrlArchivoPublicacion.Aspect = Aspect.Fill;
                UrlArchivoPublicacion.Source = App.Current.Resources["RutaDefaultVideo"] as string;
            }
            else
            {
                UrlArchivoPublicacion.Source = item.UrlArchivoPublicacion;
            }
        }

        private void InteracturarPublicacion_Tapped(object sender, EventArgs e)
        {
            if (InteracturarPublicacion != null)
            {
                InteracturarPublicacion.Execute(BindingContext);
            }
        }

        private void InteracturarIconoPublicacion_Tapped(object sender, EventArgs e)
        {
            if (InteracturarIconoPublicacion != null)
            {
                InteracturarIconoPublicacion.Execute(BindingContext);
            }
        }

        private void IrPersona_Tapped(object sender, EventArgs e)
        {
            if (IrPersona != null)
            {
                IrPersona.Execute(BindingContext);
            }
        }
    }
}