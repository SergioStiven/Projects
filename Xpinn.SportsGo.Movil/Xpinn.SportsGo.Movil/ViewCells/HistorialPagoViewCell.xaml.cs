using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Models;

namespace Xpinn.SportsGo.Movil.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistorialPagoViewCell : ViewCell
	{
		public HistorialPagoViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty RegistroSeleccionadoProperty =
	        BindableProperty.Create(
	            nameof(RegistroSeleccionado),
	            typeof(ICommand),
	            typeof(HistorialPagoViewCell),
	            default(ICommand),
	            BindingMode.TwoWay
	        );

	    public ICommand RegistroSeleccionado
	    {
	        get { return (ICommand)GetValue(RegistroSeleccionadoProperty); }
	        set { SetValue(RegistroSeleccionadoProperty, value); }
	    }

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        HistorialPagosModel item = BindingContext as HistorialPagosModel;

	        if (item == null) return;

	        UrlArchivo.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.HistorialPago.Planes.UrlArchivo))
	        {
	            UrlArchivo.Source = item.HistorialPago.Planes.UrlArchivo;
	        }
	        else
	        {
	            UrlArchivo.Source = App.Current.Resources["RutaDefaultImagen"] as string;
	        }
	    }

        private void RegistroSeleccionado_OnTapped(object sender, EventArgs e)
	    {
	        if (RegistroSeleccionado != null)
	        {
	            RegistroSeleccionado.Execute(BindingContext);
            }
	    }
	}
}