using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Movil.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListaPlanesViewCell : ViewCell
	{
		public ListaPlanesViewCell ()
		{
			InitializeComponent ();
		}

	    public static readonly BindableProperty RegistroSeleccionadoProperty =
	        BindableProperty.Create(
	            nameof(RegistroSeleccionado),
	            typeof(ICommand),
	            typeof(ListaPlanesViewCell),
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
	        PlanesDTO item = BindingContext as PlanesDTO;

	        if (item == null) return;

	        UrlArchivo.Source = null;
	        if (!string.IsNullOrWhiteSpace(item.UrlArchivo))
	        {
	            UrlArchivo.Source = item.UrlArchivo;
	        }
	        else
	        {
	            UrlArchivo.Source = App.Current.Resources["RutaDefaultImagen"] as string;
	        }

	        AbreviaturaMoneda.Text = "(" + App.Persona.Paises.Monedas.AbreviaturaMoneda + "): ";
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