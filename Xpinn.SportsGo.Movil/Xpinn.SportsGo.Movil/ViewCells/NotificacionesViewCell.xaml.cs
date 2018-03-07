using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificacionesViewCell : ViewCell
	{
		public NotificacionesViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty InteractuarNotificacionProperty =
	        BindableProperty.Create(
	            nameof(InteractuarNotificacion),
	            typeof(ICommand),
	            typeof(NotificacionesViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand InteractuarNotificacion
        {
	        get { return (ICommand)GetValue(InteractuarNotificacionProperty); }
	        set { SetValue(InteractuarNotificacionProperty, value); }
	    }

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        TimeLineNotificaciones item = BindingContext as TimeLineNotificaciones;

	        if (item == null) return;

            UrlArchivo.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.UrlArchivo))
	        {
	            UrlArchivo.Source = item.UrlArchivo;
	        }
	        else
	        {
	            UrlArchivo.Source = App.Current.Resources["RutaDefaultImagenPerfil"] as string;
	        }
	    }

	    private void InteractuarNotificacion_Tapped(object sender, EventArgs e)
	    {
	        if (InteractuarNotificacion != null)
	        {
	            InteractuarNotificacion.Execute(BindingContext);
	        }
	    }
    }
}